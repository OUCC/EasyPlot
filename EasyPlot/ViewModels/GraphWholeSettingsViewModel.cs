using CommunityToolkit.Mvvm.ComponentModel;
using EasyPlot.ViewModels.Wrapper;

namespace EasyPlot.ViewModels;

public class GraphWholeSettingsViewModel: ObservableObject
{
    public WholeSettings WholeSettings { get; }

    public GraphWholeSettingsViewModel(MainViewModel mainViewModel)
    {
        WholeSettings = mainViewModel.WholeSettings;
    }
}
