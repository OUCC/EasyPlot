using EasyPlot.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace EasyPlot.Views;

public sealed partial class MainPage : Page
{
    public MainViewModel ViewModel { get; }

    public MainPage()
    {
        ViewModel = App.GetService<MainViewModel>();
        InitializeComponent();
        ViewModel.OnComponentInitialized();
    }
}
