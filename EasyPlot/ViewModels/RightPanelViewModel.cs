using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasyPlot.Contracts.Services;
using Windows.Storage.Pickers;
using Windows.Storage;
using WinRT.Interop;

namespace EasyPlot.ViewModels;

public partial class RightPanelViewModel : ObservableObject
{
    public MainViewModel MainViewModel { get; }

    private readonly ISharedConfigService _sharedConfigService;

    public RightPanelViewModel(MainViewModel mainViewModel, ISharedConfigService sharedConfigService)
    {
        MainViewModel = mainViewModel;
        _sharedConfigService = sharedConfigService;
    }

    [RelayCommand]
    private async Task OnSaveImageAsync()
    {
        if (!File.Exists(_sharedConfigService.ResultPngPath))
            return;

        var copiedPath = Path.Combine(_sharedConfigService.TempDirectory, "result-copied.png");
        File.Copy(_sharedConfigService.ResultPngPath, copiedPath, true);

        var picker = new FileSavePicker();
        picker.FileTypeChoices.Add("png", new[] { ".png" });
        picker.SuggestedFileName = "result";

        var hwnd = WindowNative.GetWindowHandle(App.MainWindow);
        InitializeWithWindow.Initialize(picker, hwnd);
        var file = await picker.PickSaveFileAsync();

        if (file is null)
            return;

        CachedFileManager.DeferUpdates(file);
        File.Move(copiedPath, file.Path, true);
        _ = await CachedFileManager.CompleteUpdatesAsync(file);
    }
}
