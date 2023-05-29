using CommunityToolkit.Mvvm.ComponentModel;

namespace EasyPlot.ViewModels.Values;

internal partial class WholeSettings : ObservableObject
{
    public TextValue<string> Title { get; set; } = new(string.Empty);

    public TextValue<string> XLabel { get; set; } = new(string.Empty);

    public TextValue<string> YLabel { get; set; } = new(string.Empty);

    public TextValue<string> Sampling { get; set; } = new(string.Empty);

    public RangeValue XRange { get; set; } = new();

    public RangeValue YRange { get; set; } = new();

    [ObservableProperty]
    private bool _enabledXLogscale;

    [ObservableProperty]
    private bool _enabledYLogscale;
}
