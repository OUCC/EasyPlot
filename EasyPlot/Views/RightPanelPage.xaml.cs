using EasyPlot.ViewModels;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EasyPlot.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class RightPanelPage : Page
{
    public RightPanelViewModel ViewModel { get; }

    public RightPanelPage()
    {
        ViewModel = App.GetService<RightPanelViewModel>();
        InitializeComponent();

        //ViewModel.MainViewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainViewModel.ResultImage))
        {
            //MainBitmap.UriSource = ViewModel.MainViewModel.ResultPath;
        }
    }
}
