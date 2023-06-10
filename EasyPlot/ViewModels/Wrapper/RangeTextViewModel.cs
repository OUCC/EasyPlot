using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace EasyPlot.ViewModels.Wrapper;

public partial class RangeTextViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _enabled = false;

    [ObservableProperty]
    private string _start = string.Empty;

    [ObservableProperty]
    private string _end = string.Empty;

    [RelayCommand]
    private void OnClicked()
    {
        Enabled = true;
    }
}
