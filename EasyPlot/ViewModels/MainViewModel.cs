using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasyPlot.Contracts.Services;
using EasyPlot.Utilities;
using EasyPlot.ViewModels.Wrapper;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;

namespace EasyPlot.ViewModels;

public partial class MainViewModel : ObservableRecipient
{
    public static Encoding UTF8WithoutBOM { get; } = new UTF8Encoding(false);

    public WholeSettings WholeSettings { get; } = new();

    public List<GraphGroupViewModel> GraphGroups { get; }

    public ObservableCollection<MainTabViewModel> TabItems { get; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsGenerateError))]
    [NotifyPropertyChangedFor(nameof(IsGenerateSuccess))]
    private string _errorText = string.Empty;

    public bool IsGenerateError => ErrorText != string.Empty;

    public bool IsGenerateSuccess => ErrorText == string.Empty;

    public ImageSource? ResultImage { get; private set; }

    public Uri ResultPath => new Uri(_sharedConfigService.ResultPngPath);

    private readonly CancellationTokenSource _cts = new();

    private readonly PeriodicTimer _loopTimer;

    private string? _lastPlotText;

    private readonly INavigationService _navigationService;

    private readonly ISharedConfigService _sharedConfigService;

    public MainViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
        _sharedConfigService = App.GetService<ISharedConfigService>();

        GraphGroups = new(new[] { new GraphGroupViewModel() { GroupTitle = "Group 1" } });
        TabItems = new();

        _loopTimer = new PeriodicTimer(TimeSpan.FromSeconds(1));
        PlotImageLoopAsync();
    }

    #region commands
    public void OnAddTabButtonClick(TabView sender, object _)
    {
        // タイトルをBindするかが異なるため、直接バインディング
        var newGroup = new GraphGroupViewModel()
        {
            GroupTitle = $"Group {GraphGroups.Count + 1}",
        };
        GraphGroups.Add(newGroup);
        TabItems.Add(new MainTabViewModel(newGroup));

        sender.SelectedIndex = TabItems.Count - 1;
    }

    public void OnTabCloseRequested(TabView _, TabViewTabCloseRequestedEventArgs args)
    {
        var item = (MainTabViewModel)args.Item;
        TabItems.Remove(item);
        if (item.Data is not null)
        {
            GraphGroups.Remove(item.Data);

            if (GraphGroups.Count == 0)
            {
                var newGroup = new GraphGroupViewModel()
                {
                    GroupTitle = "Group 1",
                };
                GraphGroups.Add(newGroup);
                TabItems.Add(new MainTabViewModel(newGroup));
            }
        }
    }

    public void OnComponentInitialized()
    {
        // なかで ViewModel を要求するので初期化後に呼ぶ
        TabItems.Add(new MainTabViewModel(WholeSettings));
        TabItems.Add(new MainTabViewModel(GraphGroups[0]));
    }

    [RelayCommand]
    private void OnOpenSettingTab(TabView tabView)
    {
        var settingTab = TabItems
            .Select((tab, index) => (tab, index))
            .FirstOrDefault(t => t.tab.TabType == MainTabType.AppSettings);
        if (settingTab.tab is null)
        {
            settingTab = (MainTabViewModel.CreateSetting(), TabItems.Count);

            TabItems.Add(settingTab.tab);
        }

        tabView.SelectedIndex = settingTab.index;
    }

    [RelayCommand]
    private void OnSettingClicked()
    {
        _navigationService.NavigateTo(typeof(SettingsViewModel).FullName!);
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
            await Task.Delay(1000);

            if (!File.Exists(_sharedConfigService.ResultPngPath))
                using (File.Create(_sharedConfigService.ResultPngPath)) { }

            while (await _loopTimer.WaitForNextTickAsync(token))
            {
                try
                {
                    token.ThrowIfCancellationRequested();
                    var plotText = GeneratePlotText();

                    if (plotText != _lastPlotText)
                    {
                        _lastPlotText = plotText;
                        await CreatePlotImageAsync(plotText, token);
                    }
                    else
                    {
                        _lastPlotText = plotText;
                    }
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
            ErrorText = ex.ToString();
        }
    }

    private async Task CreatePlotImageAsync(string plotText, CancellationToken cancellationToken)
    {
        var basePath = _sharedConfigService.TempDirectory;
        var gpFilePath = _sharedConfigService.ResultGpPath;
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

        var imageFile = await StorageFile.GetFileFromPathAsync(_sharedConfigService.ResultPngPath);
        var stream = await imageFile.OpenReadAsync();
        var bitmap = new BitmapImage();
        await bitmap.SetSourceAsync(stream);
        ResultImage = bitmap;
        OnPropertyChanged(nameof(ResultPath));
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
            set output "{_sharedConfigService.ResultPngPathNotBackSlash}"
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
