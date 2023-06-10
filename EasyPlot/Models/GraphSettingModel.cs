using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasyPlot.ViewModels.Wrapper;
using Windows.Storage.Pickers;

namespace EasyPlot.Models;

public partial class GraphSettingModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsDataFile))]
    private bool _isFunction = true;

    public bool IsDataFile => !IsFunction;

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
        picker.FileTypeFilter.Add("*.txt, *.tsv");
        var result = await picker.PickSingleFileAsync();
        if (result is null)
            return;

        DataFilePath = result.Path;
        IsFunction = false;
    }

    [RelayCommand]
    private void OnFunctionTextBoxClicked()
    {
        IsFunction = true;
    }
}
