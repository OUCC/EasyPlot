using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace EasyPlot.ViewModels.Values;

internal partial class GraphSettings : ObservableObject
{
    private static int _idSource = 0;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsDataFile))]
    [NotifyPropertyChangedFor(nameof(FunctionBackgroundColor))]
    private bool _isFunction = true;

    public bool IsDataFile => !IsFunction;

    [ObservableProperty]
    private string _functionText = string.Empty;

    partial void OnFunctionTextChanged(string value) => IsFunction = true;

    public Color FunctionBackgroundColor => IsFunction ? Colors.White : Colors.WhiteSmoke;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DataFileName))]
    private string _dataFilePath = string.Empty;

    public string DataFileName => Path.GetFileName(DataFilePath);

    [ObservableProperty]
    private RangeValue _usingRange = new();

    [ObservableProperty]
    private TextValue<string> _title = new(string.Empty);

    public int Id { get; } = _idSource++;

    public string RadioGroupName { get; }

    public GraphSettings()
    {
        RadioGroupName = $"FunctionOrDataFile{Id}";
    }

    [RelayCommand]
    private async Task OnOpenFilePicker()
    {
        var result = await FilePicker.Default.PickAsync();
        if (result is null)
            return;

        DataFilePath = result.FullPath;
    }
}
