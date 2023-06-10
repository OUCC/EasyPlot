using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace EasyPlot.ViewModels.Wrapper;

public partial class TextBoxViewModel<T> : ObservableObject
{
    [ObservableProperty]
    private T _value;

    [ObservableProperty]
    private bool _enabled = false;

    public TextBoxViewModel(T initValue)
    {
        _value = initValue;
    }

    [RelayCommand]
    private void OnClicked()
    {
        Enabled = true;
    }
}

