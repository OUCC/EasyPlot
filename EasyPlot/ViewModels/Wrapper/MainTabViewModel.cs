using System.ComponentModel;
using EasyPlot.Views;
using Microsoft.UI.Xaml.Controls;

namespace EasyPlot.ViewModels.Wrapper;

public class MainTabViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public MainTabType TabType { get; }

    public GraphGroupViewModel? Data { get; }

    public string Title { get; private set; }

    public IconSource? IconSource { get; }

    public bool IsCloasble => TabType != MainTabType.GraphWholeSettings;

    public object DataContent { get; }

    private MainTabViewModel(MainTabType type, string title, object dataContent, IconSource? iconSource)
    {
        TabType = type;
        Title = title;
        DataContent = dataContent;
        IconSource = iconSource;
    }

    public MainTabViewModel(WholeSettings _)
    {
        TabType = MainTabType.GraphWholeSettings;
        Title = "Home";

        IconSource = new SymbolIconSource { Symbol = Symbol.Home };

        var frame = new Frame();
        frame.Navigate(typeof(GraphWholeSettingsPage));
        DataContent = frame;
    }

    public MainTabViewModel(GraphGroupViewModel viewModel)
    {
        TabType = MainTabType.GraphGroup;
        Title = viewModel.GroupTitle;
        viewModel.PropertyChanged += OnTitleChanged;

        Data = viewModel;
        var frame = new Frame();
        frame.Navigate(typeof(GraphGroupPage), viewModel);
        DataContent = frame;
    }

    public static MainTabViewModel CreateSetting()
    {
        var frame = new Frame();
        frame.Navigate(typeof(SettingsPage));
        return new MainTabViewModel(MainTabType.AppSettings, "Setting", frame, new SymbolIconSource { Symbol = Symbol.Setting });
    }

    private void OnTitleChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (PropertyChanged is null || e.PropertyName != nameof(GraphGroupViewModel.GroupTitle))
            return;

        Title = Data?.GroupTitle ?? string.Empty;
        PropertyChanged(this, TitleArgs);
    }

    private static readonly PropertyChangedEventArgs TitleArgs = new(nameof(Title));
}
