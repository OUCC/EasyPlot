using EasyPlot.ViewModels;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EasyPlot.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class GraphWholeSettingsPage : Page
{
    public GraphWholeSettingsViewModel ViewModel { get; }

    public GraphWholeSettingsPage()
    {
        ViewModel = App.GetService<GraphWholeSettingsViewModel>();
        InitializeComponent();
        ImagePane.Navigate(typeof(RightPanelPage));
    }
}
