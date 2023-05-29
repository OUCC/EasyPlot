using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace EasyPlot.ViewModels.Values;

internal partial class GraphGroup : ObservableObject
{
    private static int _idSource = 0;

    public ObservableCollection<GraphSettings> Settings { get; set; } = new();

    public int Id { get; } = _idSource++;

    [ObservableProperty]
    private string _groupTitle = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TabBackgroundColor))]
    private bool _selected = false;

    public Color TabBackgroundColor => Selected ? MyColors.TabBackGround : Colors.LightGray;

    [ObservableProperty]
    private bool _isWithLines = false;

    public TextValue<string> LineWidth { get; set; } = new(string.Empty);

    public TextValue<string> LineType { get; set; } = new(string.Empty);

    [ObservableProperty]
    private bool _isWithPoints = false;

    public TextValue<string> PointsSize { get; set; } = new(string.Empty);

    public TextValue<string> PointsType { get; set; } = new(string.Empty);

    public void RegisterPropertyChanged(PropertyChangedEventHandler eventHandler)
    {
        PropertyChanged += eventHandler;
        LineWidth.PropertyChanged += eventHandler;
        LineType.PropertyChanged += eventHandler;
        PointsSize.PropertyChanged += eventHandler;
        PointsType.PropertyChanged += eventHandler;
    }
}
