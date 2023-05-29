using System.Windows.Input;

namespace EasyPlot.Views;

public partial class TabButton : ContentView
{
    private static readonly BindableProperty _tabTextProperty = BindableProperty.Create(nameof(TabText), typeof(string), typeof(TabButton), null);

    private static readonly BindableProperty _isSelectedProperty = BindableProperty.Create(nameof(IsSelected), typeof(bool), typeof(TabButton), false);

    private static readonly BindableProperty _showCloseProperty = BindableProperty.Create(nameof(ShowClose), typeof(bool), typeof(TabButton), true);

    private static readonly BindableProperty _tabWidthProperty = BindableProperty.Create(nameof(TabWidth), typeof(double), typeof(TabButton), 0);

    private static readonly BindableProperty _closeCommandPropery = BindableProperty.Create(nameof(CloseCommand), typeof(ICommand), typeof(TabButton), null);

    private static readonly BindableProperty _closeCommandParameterPropery = BindableProperty.Create(nameof(CloseCommandParameter), typeof(object), typeof(TabButton), null);

    private static readonly BindableProperty _clickCommandPropery = BindableProperty.Create(nameof(ClickCommand), typeof(ICommand), typeof(TabButton), null);

    private static readonly BindableProperty _clickCommandParameterPropery = BindableProperty.Create(nameof(ClickCommandParameter), typeof(object), typeof(TabButton), null);

    #region bindable properties
    public string? TabText
    {
        get => (string?)GetValue(_tabTextProperty);
        set => SetValue(_tabTextProperty, value);
    }

    public bool IsSelected
    {
        get => (bool)GetValue(_isSelectedProperty);
        set
        {
            SetValue(_isSelectedProperty, value);
            OnPropertyChanged(nameof(TabBackgroundColor));
        }
    }

    public bool ShowClose
    {
        get => (bool)GetValue(_showCloseProperty);
        set => SetValue(_showCloseProperty, value);
    }

    public double TabWidth
    {
        get => (double)GetValue(_tabWidthProperty);
        set
        {
            SetValue(_tabWidthProperty, value);
            OnPropertyChanged(nameof(ColumnDefinition));
        }
    }

    public object? CloseCommandParameter
    {
        get => GetValue(_closeCommandParameterPropery);
        set => SetValue(_closeCommandParameterPropery, value);
    }

    public ICommand? CloseCommand
    {
        get => (ICommand?)GetValue(_closeCommandPropery);
        set => SetValue(_closeCommandPropery, value);
    }

    public object? ClickCommandParameter
    {
        get => GetValue(_clickCommandParameterPropery);
        set => SetValue(_clickCommandParameterPropery, value);
    }

    public ICommand? ClickCommand
    {
        get => (ICommand?)GetValue(_clickCommandPropery);
        set => SetValue(_clickCommandPropery, value);
    }
    #endregion

    private Color TabBackgroundColor => IsSelected ? Colors.WhiteSmoke : Colors.LightGray;

    private ColumnDefinitionCollection ColumnDefinition
    {
        get
        {
            _columnDefinitions[0].Width = new(TabWidth < 2 ? TabWidth : TabWidth - 2, GridUnitType.Absolute);
            return _columnDefinitions;
        }
    }
    private readonly ColumnDefinitionCollection _columnDefinitions;

    public TabButton()
    {
        _columnDefinitions = new ColumnDefinitionCollection()
        {
            new() { Width = new(TabWidth < 2 ? TabWidth : TabWidth - 2, GridUnitType.Absolute) },
            new() { Width = new(35, GridUnitType.Absolute) }
        };
        InitializeComponent();
    }
}
