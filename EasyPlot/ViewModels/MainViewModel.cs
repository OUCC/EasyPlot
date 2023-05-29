using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasyPlot.Utilities;
using EasyPlot.ViewModels.Values;
using Microsoft.Maui.Storage;

namespace EasyPlot.ViewModels;

internal sealed partial class MainViewModel : ObservableObject, IDisposable
{
    public static Encoding UTF8WithoutBOM { get; } = new UTF8Encoding(false);


    public ReadOnlyCollection<WholeSettings> WholeSettinsEnumerable { get; }

    public WholeSettings WholeSettings { get; } = new();

    public ObservableCollection<GraphGroup> GraphGroups { get; set; }

    public GraphGroup SelectedGroup
    {
        get
        {
            var group = GraphGroups.FirstOrDefault(g => g.Selected);
            if (group is null)
            {
                if (GraphGroups.Any())
                {
                    group = GraphGroups.First();
                    IsWholeSetting = true;
                }
                else
                {
                    group = new GraphGroup
                    {
                        GroupTitle = "Group 1",
                    };
                    GraphGroups.Add(group);
                    IsWholeSetting = true;
                }
            }
            return group!;
        }
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsGraphSetting))]
    [NotifyPropertyChangedFor(nameof(WholeSettingColor))]
    [NotifyPropertyChangedFor(nameof(Title))]
    private bool _isWholeSetting = true;

    public bool IsGraphSetting => !IsWholeSetting;

    public Color WholeSettingColor => IsWholeSetting ? Colors.WhiteSmoke : Colors.LightGray;

    public string Title => IsWholeSetting ? "Whole Setting" : "Graph Setting";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsGenerateError))]
    [NotifyPropertyChangedFor(nameof(IsGenerateSuccess))]
    private string _errorText = string.Empty;

    public bool IsGenerateError => ErrorText != string.Empty;

    public bool IsGenerateSuccess => ErrorText == string.Empty;

    public string ResultImagePath { get; set; } = Path.Combine(FileSystem.Current.CacheDirectory, "result.png");

    public ImageSource? ResultImage { get; private set; }

    private readonly object _lockObj = new();

    private readonly CancellationTokenSource _cts = new();

    private readonly PeriodicTimer _loopTimer;

    private string? _lastPlotText;

    public MainViewModel()
    {
        WholeSettinsEnumerable = new(new[] { WholeSettings });

        var g = new GraphGroup() { GroupTitle = $"Group 1" };
        GraphGroups = new(new[] { g });

        _loopTimer = new PeriodicTimer(TimeSpan.FromSeconds(2));
        PlotImageLoopAsync();
    }

    #region commands
    [RelayCommand]
    private void OnWholeSetting()
    {
        IsWholeSetting = true;
        foreach (var group in GraphGroups)
        {
            group.Selected = false;
        }
    }

    [RelayCommand]
    private void OnAddGroup()
    {
        var g = new GraphGroup()
        {
            GroupTitle = $"Group {GraphGroups.Count + 1}"
        };
        GraphGroups.Add(g);
    }

    [RelayCommand]
    private void OnRemoveGroup(int id)
    {
        var group = GraphGroups.FirstOrDefault(g => g.Id == id);
        if (group is null)
            return;

        GraphGroups.Remove(group);
        OnPropertyChanged(nameof(SelectedGroup));
    }

    [RelayCommand]
    private void OnSelectGroup(int id)
    {
        foreach (var group in GraphGroups)
        {
            group.Selected = group.Id == id;
        }
        IsWholeSetting = false;
        OnPropertyChanged(nameof(SelectedGroup));
    }

    [RelayCommand]
    private void OnAddGraphSetting()
    {
        var s = new GraphSettings();
        SelectedGroup.Settings.Add(s);
    }

    [RelayCommand]
    private void OnRemoveGraphSetting(int id)
    {
        var s = SelectedGroup.Settings.FirstOrDefault(s => s.Id == id);
        if (s is null)
            return;

        SelectedGroup.Settings.Remove(s);
    }

    [RelayCommand]
    private async Task OnSaveImageAsync()
    {
        var copiedPath = Path.Combine(FileSystem.CacheDirectory, "result-copied.png");
        File.Copy(ResultImagePath, copiedPath, true);

        using var fs = new FileStream(copiedPath, FileMode.Open);
        var options = new PickOptions
        {
            FileTypes = FilePickerFileType.Png
        };
        var saveResult = await FileSaver.Default.SaveAsync("result.png", fs, _cts.Token);
    }
    #endregion

    public void Dispose()
    {
        _cts.Cancel();
        _loopTimer.Dispose();
    }

    #region image generater
    private async void PlotImageLoopAsync()
    {
        var token = _cts.Token;

        try
        {
            // 起動直後はメインスレッドが確定していないので待つ
            await Task.Delay(2000);

            if (!File.Exists(ResultImagePath))
                using (File.Create(ResultImagePath)) { }

            while (await _loopTimer.WaitForNextTickAsync(token))
            {
                try
                {
                    token.ThrowIfCancellationRequested();
                    var plotText = GeneratePlotText();

                    if (plotText != _lastPlotText)
                    {
                        await CreatePlotImageAsync(plotText, token);
                    }
                    _lastPlotText = plotText;
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    private async Task CreatePlotImageAsync(string plotText, CancellationToken cancellationToken)
    {
        var basePath = FileSystem.Current.CacheDirectory;
        var gpFilePath = Path.Combine(basePath, "result.gp");
        await File.WriteAllTextAsync(gpFilePath, plotText, UTF8WithoutBOM, cancellationToken);
        var info = new ProcessStartInfo
        {
            CreateNoWindow = true,
            FileName = "gnuplot",
            Arguments = $"-c \"{gpFilePath}\"",
            WorkingDirectory = basePath,
            UseShellExecute = false,
            RedirectStandardError = true,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
        };
        var process = Process.Start(info);
        if (process is null)
            return;

        await process.WaitForExitAsync(cancellationToken);

        var errorText = await process.StandardOutput.ReadLineAsync(cancellationToken) ?? string.Empty;

        lock (_lockObj)
        {
            if (process.ExitCode != 0 && errorText == string.Empty)
            {
                ErrorText = "unknown error occured";
                return;
            }
            else
            {
                ErrorText = errorText;
            }
        }

        ResultImage = ImageSource.FromFile(ResultImagePath);
        OnPropertyChanged(nameof(ResultImage));
        OnPropertyChanged(nameof(ErrorText));
        OnPropertyChanged(nameof(IsGenerateError));
        OnPropertyChanged(nameof(IsGenerateSuccess));
    }

    private string GeneratePlotText()
    {
        using var builder = new ValueStringBuilder();
        builder.Append($"""
            set encoding utf8
            set terminal pngcairo
            set output "{ResultImagePath.Replace('\\', '/')}"
            """);

        if (WholeSettings.Title.Enabled)
            builder.Append($"\nset title \"{WholeSettings.Title.Value}\"");
        if (WholeSettings.XLabel.Enabled)
            builder.Append($"\nset xlabel \"{WholeSettings.XLabel.Value}\"");
        if (WholeSettings.YLabel.Enabled)
            builder.Append($"\nset ylabel \"{WholeSettings.YLabel.Value}\"");
        if (WholeSettings.XRange.Enabled)
            builder.Append($"\nset xrange [{WholeSettings.XRange.Start}:{WholeSettings.XRange.End}]");
        if (WholeSettings.YRange.Enabled)
            builder.Append($"\nset yrange [{WholeSettings.YRange.Start}:{WholeSettings.YRange.End}]");
        if (WholeSettings.EnabledXLogscale)
            builder.Append("\nset logscale x");
        if (WholeSettings.EnabledYLogscale)
            builder.Append("\nset logscale y");
        if (WholeSettings.Sampling.Enabled)
            builder.Append($"\nset sample {WholeSettings.Sampling.Value}");

        if (GraphGroups.Any(g => g.Settings.Any()))
            builder.Append("\nplot ");

        foreach (var g in GraphGroups)
        {
            foreach (var s in g.Settings)
            {
                if (s.IsFunction)
                    builder.Append($"{s.FunctionText} ");
                else
                {
                    builder.Append($"\"{s.DataFilePath}\" ");
                    if (s.UsingRange.Enabled)
                        builder.Append($"using {s.UsingRange.Start}:{s.UsingRange.End} ");
                }

                if (s.Title.Enabled)
                    builder.Append($"title \"{s.Title.Value}\" ");

                if (g.IsWithLines)
                {
                    builder.Append("with lines ");
                    if (g.LineType.Enabled)
                        builder.Append($"linetype {g.LineType.Value} ");
                    if (g.LineWidth.Enabled)
                        builder.Append($"linewith {g.LineWidth.Value} ");
                }

                if (g.IsWithPoints)
                {
                    builder.Append("with points ");
                    if (g.PointsType.Enabled)
                        builder.Append($"pointtype {g.PointsType.Value} ");
                    if (g.PointsSize.Enabled)
                        builder.Append($"pointsize {g.PointsSize.Value} ");
                }

                builder.Append("\\\n, ");
            }
        }
        var temp = builder.GetRawSpan();
        if (temp.EndsWith("\\\n, "))
        {
            return temp[..^4].ToString();
        }
        else
        {
            return temp.ToString();
        }
    }
    #endregion
}
