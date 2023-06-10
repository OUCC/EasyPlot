using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasyPlot.Contracts.Services;
using EasyPlot.Utilities;
using EasyPlot.ViewModels.Wrapper;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace EasyPlot.ViewModels;

public partial class MainViewModel : ObservableRecipient
{
    public static Encoding UTF8WithoutBOM { get; } = new UTF8Encoding(false);

    public WholeSettings WholeSettings { get; } = new();

    public ObservableCollection<ObservableObject> TabItems { get; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsGenerateError))]
    [NotifyPropertyChangedFor(nameof(IsGenerateSuccess))]
    private string _errorText = string.Empty;

    public bool IsGenerateError => ErrorText != string.Empty;

    public bool IsGenerateSuccess => ErrorText == string.Empty;

    public string ResultImagePath { get; set; } = Path.Combine(Path.GetTempPath(), "EasyPlot", "result.png");

    public ImageSource? ResultImage { get; private set; }

    private readonly object _lockObj = new();

    private readonly CancellationTokenSource _cts = new();

    private readonly PeriodicTimer _loopTimer;

    private string? _lastPlotText;

    private readonly INavigationService _navigationService;

    public MainViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;

        TabItems = new(new ObservableObject[] { WholeSettings, new GraphGroupViewModel() { GroupTitle = "Group 1" } });

        _loopTimer = new PeriodicTimer(TimeSpan.FromSeconds(1));
        PlotImageLoopAsync();
    }

    #region commands

    public void OnAddTabButtonClick(TabView sender, object args)
    {
        TabItems.Add(new GraphGroupViewModel()
        {
            GroupTitle = $"Group {TabItems.Count + 1}",
        });
    }

    public void OnTabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
    {
        TabItems.Remove((args.Item as ObservableObject)!);
        if (TabItems.Count < 2)
        {
            TabItems.Add(new GraphGroupViewModel { GroupTitle = "Group 1" });
        }
    }

    [RelayCommand]
    private void OnSettingClicked()
    {
        _navigationService.NavigateTo(typeof(SettingsViewModel).FullName!);
    }

    [RelayCommand]
    private async Task OnSaveImageAsync()
    {
        var copiedPath = Path.Combine(Path.GetTempPath(), "EasyPlot", "result-copied.png");
        File.Copy(ResultImagePath, copiedPath, true);

        var picker = new FileSavePicker();
        picker.FileTypeChoices.Add("png", new[] { ".png" });
        picker.SuggestedFileName = "result";
        var file = await picker.PickSaveFileAsync();

        if (file is null)
            return;

        CachedFileManager.DeferUpdates(file);
        File.Copy(copiedPath, file.Path);
        _ = await CachedFileManager.CompleteUpdatesAsync(file);
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
                    ErrorText = ex.ToString();
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
        var basePath = Path.Combine(Path.GetTempPath(), "EasyPlot");
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

        var errorText = await process.StandardError.ReadLineAsync(cancellationToken) ?? string.Empty;
        await process.WaitForExitAsync(cancellationToken);

        if (errorText != string.Empty)
            errorText = errorText.Replace($"\"{gpFilePath}\"", string.Empty);

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
                if (IsGenerateError)
                    return;
            }
        }

        //ResultImage = ImageSource.FromFile(ResultImagePath);
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

        var graphGroups = TabItems.OfType<GraphGroupViewModel>().ToArray();

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

        if (graphGroups.Any(g => g.Settings.Any()))
            builder.Append("\nplot ");

        foreach (var g in graphGroups)
        {
            foreach (var s in g.Settings)
            {
                if (s.IsFunction)
                    builder.Append($"{s.FunctionText} ");
                else
                {
                    builder.Append($"\"{s.DataFilePath.Replace('\\', '/')}\" ");
                    if (s.UsingRange.Enabled)
                        builder.Append($"using {s.UsingRange.Start}:{s.UsingRange.End} ");
                }

                if (s.Title.Enabled)
                    builder.Append($"title \"{s.Title.Value}\" ");

                if (g.IsWithLines && g.IsWithPoints)
                {
                    builder.Append("with linespoints ");
                    if (g.LineType.Enabled)
                        builder.Append($"linetype {g.LineType.Value} ");
                    if (g.LineWidth.Enabled)
                        builder.Append($"linewith {g.LineWidth.Value} ");
                    if (g.PointsType.Enabled)
                        builder.Append($"pointtype {g.PointsType.Value} ");
                    if (g.PointsSize.Enabled)
                        builder.Append($"pointsize {g.PointsSize.Value} ");
                }
                else if (g.IsWithLines)
                {
                    builder.Append("with lines ");
                    if (g.LineType.Enabled)
                        builder.Append($"linetype {g.LineType.Value} ");
                    if (g.LineWidth.Enabled)
                        builder.Append($"linewith {g.LineWidth.Value} ");
                }
                else if (g.IsWithPoints)
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
