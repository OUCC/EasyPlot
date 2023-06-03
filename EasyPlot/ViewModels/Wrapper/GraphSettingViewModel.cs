using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasyPlot.Models;

namespace EasyPlot.ViewModels.Wrapper;

internal partial class GraphSettingViewModel : ObservableObject
{
    public GraphSettingViewModel(GraphSettingModel model)
    {
        Model = model;
    }

    public GraphSettingModel Model { get; set; }

    #region Notifiers
    public bool TitleEnabled
    {
        get => Model.Title.Enabled;
        set
        {
            if (Model.Title.Enabled != value)
            {
                Model.Title.Enabled = value;
                OnPropertyChanged(EventArgs.TitleEnabled);
                OnPropertyChanged(EventArgs.TitleValueColor);
            }
        }
    }

    public string TitleValue
    {
        get => Model.Title.Value;
        set
        {
            if (Model.Title.Value != value)
            {
                Model.Title.Value = value;
                OnPropertyChanged(EventArgs.TitleValue);
                TitleEnabled = true;
            }
        }
    }

    public Color TitleValueColor => TitleEnabled ? ThemeColors.TextBoxEnabled : ThemeColors.TextBoxDisabled;

    public bool IsFunction
    {
        get => Model.IsFunction;
        set
        {
            if (Model.IsFunction != value)
            {
                Model.IsFunction = value;
                OnPropertyChanged(EventArgs.IsFunction);
                OnPropertyChanged(EventArgs.IsDataFile);
                OnPropertyChanged(EventArgs.FunctionTextColor);
            }
        }
    }

    public bool IsDataFile
    {
        get => !IsFunction;
        set => IsFunction = !value;
    }

    public string FunctionText
    {
        get => Model.FunctionText;
        set
        {
            if (Model.FunctionText != value)
            {
                Model.FunctionText = value;
                OnPropertyChanged(EventArgs.FunctionText);
                IsFunction = true;
            }
        }
    }

    public Color FunctionTextColor => IsFunction ? ThemeColors.TextBoxEnabled : ThemeColors.TextBoxDisabled;

    public string DataFilePath
    {
        get => Model.DataFilePath;
        set
        {
            if (Model.DataFilePath != value)
            {
                Model.DataFilePath = value;
                OnPropertyChanged(EventArgs.DataFilePath);
                OnPropertyChanged(EventArgs.DataFileName);
            }
        }
    }

    public string DataFileName => Path.GetFileName(DataFilePath);

    public bool UsingRangeEnabled
    {
        get => Model.UsingRange.Enabled;
        set
        {
            if (Model.UsingRange.Enabled != value)
            {
                Model.UsingRange.Enabled = value;
                OnPropertyChanged(EventArgs.UsingRangeEnabled);
                OnPropertyChanged(EventArgs.UsingRangeTextColor);
            }
        }
    }

    public Color UsingRangeTextColor => UsingRangeEnabled && IsDataFile ? ThemeColors.TextBoxEnabled : ThemeColors.TextBoxDisabled;

    public string UsingRangeStart
    {
        get => Model.UsingRange.Start;
        set
        {
            if (Model.UsingRange.Start != value)
            {
                Model.UsingRange.Start = value;
                OnPropertyChanged(EventArgs.UsingRangeStart);
                UsingRangeEnabled = true;
            }
        }
    }

    public string UsingRangeEnd
    {
        get => Model.UsingRange.End;
        set
        {
            if (Model.UsingRange.End != value)
            {
                Model.UsingRange.End = value;
                OnPropertyChanged(EventArgs.UsingRangeEnd);
                UsingRangeEnabled = true;
            }
        }
    }
    #endregion

    [RelayCommand]
    private async Task OnOpenFilePicker()
    {
        var result = await FilePicker.Default.PickAsync();
        if (result is null)
            return;

        DataFilePath = result.FullPath;
        IsDataFile = true;
    }

    private static class EventArgs
    {
        public static readonly PropertyChangedEventArgs TitleEnabled = new(nameof(GraphSettingViewModel.TitleEnabled));
        public static readonly PropertyChangedEventArgs TitleValue = new(nameof(GraphSettingViewModel.TitleValue));
        public static readonly PropertyChangedEventArgs TitleValueColor = new(nameof(GraphSettingViewModel.TitleValueColor));
        public static readonly PropertyChangedEventArgs IsFunction = new(nameof(GraphSettingViewModel.IsFunction));
        public static readonly PropertyChangedEventArgs IsDataFile = new(nameof(GraphSettingViewModel.IsDataFile));
        public static readonly PropertyChangedEventArgs FunctionText = new(nameof(GraphSettingViewModel.FunctionText));
        public static readonly PropertyChangedEventArgs FunctionTextColor = new(nameof(GraphSettingViewModel.FunctionTextColor));
        public static readonly PropertyChangedEventArgs DataFilePath = new(nameof(GraphSettingViewModel.DataFilePath));
        public static readonly PropertyChangedEventArgs DataFileName = new(nameof(GraphSettingViewModel.DataFileName));
        public static readonly PropertyChangedEventArgs UsingRangeEnabled = new(nameof(GraphSettingViewModel.UsingRangeEnabled));
        public static readonly PropertyChangedEventArgs UsingRangeTextColor = new(nameof(GraphSettingViewModel.UsingRangeTextColor));
        public static readonly PropertyChangedEventArgs UsingRangeStart = new(nameof(GraphSettingViewModel.UsingRangeStart));
        public static readonly PropertyChangedEventArgs UsingRangeEnd = new(nameof(GraphSettingViewModel.UsingRangeEnd));
    }
}
