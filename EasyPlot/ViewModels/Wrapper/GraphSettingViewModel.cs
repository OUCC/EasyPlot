using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasyPlot.Models;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace EasyPlot.ViewModels.Wrapper;

public partial class GraphSettingViewModel : ObservableObject
{
    public event Action<GraphSettingViewModel>? RemoveSettingRequested;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsDataFile))]
    private bool _isFunction = true;

    public bool IsDataFile
    {
        get => !IsFunction;
        set => IsFunction = !value;
    }

    [ObservableProperty]
    private string _functionText = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DataFileName))]
    private string _dataFilePath = string.Empty;

    public string DataFileName => Path.GetFileName(DataFilePath);

    [ObservableProperty]
    private RangeTextViewModel _usingRange = new();

    [ObservableProperty]
    private TextBoxViewModel<string> _title = new(string.Empty);

    [RelayCommand]
    private async Task OnOpenFilePicker()
    {
        var picker = new FileOpenPicker();
        picker.FileTypeFilter.Add("*");
        picker.FileTypeFilter.Add(".txt");
        picker.FileTypeFilter.Add(".tsv");

        var hwnd = WindowNative.GetWindowHandle(App.MainWindow);
        InitializeWithWindow.Initialize(picker, hwnd);
        var result = await picker.PickSingleFileAsync();
        if (result is null)
            return;

        DataFilePath = result.Path;
        IsFunction = false;
    }

    [RelayCommand]
    private void OnRemoveSettingRequested()
    {
        RemoveSettingRequested?.Invoke(this);
    }

    [RelayCommand]
    private void OnFunctionTextBoxClicked()
    {
        IsFunction = true;
    }
}
