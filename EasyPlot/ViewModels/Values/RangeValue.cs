using CommunityToolkit.Mvvm.ComponentModel;

namespace EasyPlot.ViewModels.Values;

internal partial class RangeValue : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(BackGroundColor))]
    private bool _enabled = false;

    [ObservableProperty]
    private string _start = string.Empty;

    [ObservableProperty]
    private string _end = string.Empty;

    public Color BackGroundColor => Enabled ? Colors.White : Colors.WhiteSmoke;

    partial void OnStartChanged(string value)
    {
        Enabled = true;
    }

    partial void OnEndChanged(string value)
    {
        Enabled = true;
    }
}
