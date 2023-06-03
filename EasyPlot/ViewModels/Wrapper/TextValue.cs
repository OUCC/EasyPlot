using CommunityToolkit.Mvvm.ComponentModel;

namespace EasyPlot.ViewModels.Wrapper;

internal partial class TextValue<T> : ObservableObject
{
    [ObservableProperty]
    private T _value;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(BackGroundColor))]
    private bool _enabled = false;

    public Color BackGroundColor => Enabled ? Colors.White : Colors.WhiteSmoke;

    public TextValue(T initValue)
    {
        _value = initValue;
    }

    partial void OnValueChanged(T value)
    {
        Enabled = true;
    }
}

