﻿using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace EasyPlot.ViewModels.Wrapper;

public partial class GraphGroupViewModel : ObservableObject
{
    public ObservableCollection<GraphSettingViewModel> Settings { get; set; } = new();

    [ObservableProperty]
    private string _groupTitle = string.Empty;

    [ObservableProperty]
    private bool _isWithLines = false;

    public TextBoxViewModel<string> LineWidth { get; set; } = new(string.Empty);

    public TextBoxViewModel<string> LineType { get; set; } = new(string.Empty);

    [ObservableProperty]
    private bool _isWithPoints = false;

    public TextBoxViewModel<string> PointsSize { get; set; } = new(string.Empty);

    public TextBoxViewModel<string> PointsType { get; set; } = new(string.Empty);

    [RelayCommand]
    private void OnAddGraph()
    {
        var newSetting = new GraphSettingViewModel();
        newSetting.RemoveSettingRequested += OnRemoveSettingRequested;
        Settings.Add(newSetting);
    }

    private void OnRemoveSettingRequested(GraphSettingViewModel sender)
    {
        Settings.Remove(sender);
    }
}
