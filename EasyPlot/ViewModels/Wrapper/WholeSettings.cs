using CommunityToolkit.Mvvm.ComponentModel;

namespace EasyPlot.ViewModels.Wrapper;

public partial class WholeSettings : ObservableObject
{
    public TextBoxViewModel<string> Title { get; set; } = new(string.Empty);

    public TextBoxViewModel<string> XLabel { get; set; } = new(string.Empty);

    public TextBoxViewModel<string> YLabel { get; set; } = new(string.Empty);

    public TextBoxViewModel<string> Sampling { get; set; } = new(string.Empty);

    public RangeTextViewModel XRange { get; set; } = new();

    public RangeTextViewModel YRange { get; set; } = new();

    [ObservableProperty]
    private bool _enabledXLogscale;

    [ObservableProperty]
    private bool _enabledYLogscale;
}
