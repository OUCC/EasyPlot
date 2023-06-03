using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasyPlot.Models;

namespace EasyPlot.ViewModels.Wrapper;

internal partial class GraphGroupViewModel : INotifyPropertyChanged
{
    public GraphGroupViewModel(GraphGroupModel current)
    {
        _selectedModel = current;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(PropertyChangedEventArgs eventArgs) => PropertyChanged?.Invoke(this, eventArgs);

    private void OnAllPropertyChanged()
    {
        if (PropertyChanged is null)
            return;

        PropertyChanged(this, EventArgs.SelectedModel);
        PropertyChanged(this, EventArgs.GroupTitle);
        PropertyChanged(this, EventArgs.IsWithLines);
        PropertyChanged(this, EventArgs.LineWidthEnabled);
        PropertyChanged(this, EventArgs.LineWidthValue);
        PropertyChanged(this, EventArgs.LineWidthValueColor);
        PropertyChanged(this, EventArgs.LineTypeEnabled);
        PropertyChanged(this, EventArgs.LineTypeValue);
        PropertyChanged(this, EventArgs.LineTypeValueColor);
        PropertyChanged(this, EventArgs.IsWithPoints);
        PropertyChanged(this, EventArgs.PointsSizeEnabled);
        PropertyChanged(this, EventArgs.PointsSizeValue);
        PropertyChanged(this, EventArgs.PointsSizeValueColor);
        PropertyChanged(this, EventArgs.PointsTypeEnabled);
        PropertyChanged(this, EventArgs.PointsTypeValue);
        PropertyChanged(this, EventArgs.PointsTypeValueColor);
    }

    #region Notifiers
    public GraphGroupModel SelectedModel
    {
        get => _selectedModel;
        set
        {
            if (_selectedModel != value)
            {
                _selectedModel = value;
                OnAllPropertyChanged();

                Settings.Clear();
                foreach (var setting in value.Settings)
                {
                    Settings.Add(new(setting));
                }
            }
        }
    }
    private GraphGroupModel _selectedModel;

    public ObservableCollection<GraphSettingViewModel> Settings { get; } = new();

    public string GroupTitle
    {
        get => SelectedModel.GroupTitle;
        set
        {
            if (SelectedModel.GroupTitle != value)
            {
                SelectedModel.GroupTitle = value;
                OnPropertyChanged(EventArgs.GroupTitle);
            }
        }
    }

    public bool IsWithLines
    {
        get => SelectedModel.IsWithLines;
        set
        {
            if (SelectedModel.IsWithLines != value)
            {
                SelectedModel.IsWithLines = value;
                OnPropertyChanged(EventArgs.IsWithLines);
            }
        }
    }

    public bool LineWidthEnabled
    {
        get => SelectedModel.LineWidth.Enabled;
        set
        {
            if (SelectedModel.LineWidth.Enabled != value)
            {
                SelectedModel.LineWidth.Enabled = value;
                OnPropertyChanged(EventArgs.LineWidthEnabled);
            }
        }
    }

    public string LineWidthValue
    {
        get => SelectedModel.LineWidth.Value;
        set
        {
            if (SelectedModel.LineWidth.Value != value)
            {
                SelectedModel.LineWidth.Value = value;
                OnPropertyChanged(EventArgs.LineWidthValue);
                LineWidthEnabled = true;
            }
        }
    }

    public Color LineWidthValueColor => LineWidthEnabled ? ThemeColors.TextBoxEnabled : ThemeColors.TextBoxDisabled;

    public bool LineTypeEnabled
    {
        get => SelectedModel.LineType.Enabled;
        set
        {
            if (SelectedModel.LineType.Enabled != value)
            {
                SelectedModel.LineType.Enabled = value;
                OnPropertyChanged(EventArgs.LineTypeEnabled);
            }
        }
    }

    public string LineTypeValue
    {
        get => SelectedModel.LineType.Value;
        set
        {
            if (SelectedModel.LineType.Value != value)
            {
                SelectedModel.LineType.Value = value;
                OnPropertyChanged(EventArgs.LineTypeValue);
                LineTypeEnabled = true;
            }
        }
    }

    public Color LineTypeValueColor => LineTypeEnabled ? ThemeColors.TextBoxEnabled : ThemeColors.TextBoxDisabled;

    public bool IsWithPoints
    {
        get => SelectedModel.IsWithPoints;
        set
        {
            if (SelectedModel.IsWithPoints != value)
            {
                SelectedModel.IsWithPoints = value;
                OnPropertyChanged(EventArgs.IsWithPoints);
            }
        }
    }

    public bool PointsSizeEnabled
    {
        get => SelectedModel.PointsSize.Enabled;
        set
        {
            if (SelectedModel.PointsSize.Enabled != value)
            {
                SelectedModel.PointsSize.Enabled = value;
                OnPropertyChanged(EventArgs.PointsSizeEnabled);
            }
        }
    }

    public string PointsSizeValue
    {
        get => SelectedModel.PointsSize.Value;
        set
        {
            if (SelectedModel.PointsSize.Value != value)
            {
                SelectedModel.PointsSize.Value = value;
                OnPropertyChanged(EventArgs.PointsSizeValue);
                PointsSizeEnabled = true;
            }
        }
    }

    public Color PointsSizeValueColor => PointsSizeEnabled ? ThemeColors.TextBoxEnabled : ThemeColors.TextBoxDisabled;

    public bool PointsTypeEnabled
    {
        get => SelectedModel.PointsType.Enabled;
        set
        {
            if (SelectedModel.PointsType.Enabled != value)
            {
                SelectedModel.PointsType.Enabled = value;
                OnPropertyChanged(EventArgs.PointsTypeEnabled);
            }
        }
    }

    public string PointsTypeValue
    {
        get => SelectedModel.PointsType.Value;
        set
        {
            if (SelectedModel.PointsType.Value != value)
            {
                SelectedModel.PointsType.Value = value;
                OnPropertyChanged(EventArgs.PointsTypeValue);
                PointsTypeEnabled = true;
            }
        }
    }

    public Color PointsTypeValueColor => PointsTypeEnabled ? ThemeColors.TextBoxEnabled : ThemeColors.TextBoxDisabled;
    #endregion

    [RelayCommand]
    private void OnAddSetting()
    {
        var settingModel = new GraphSettingModel();
        SelectedModel.Settings.Add(settingModel);
        Settings.Add(new(settingModel));
    }

    [RelayCommand]
    private void OnRemoveSetting(int id)
    {
        var model = SelectedModel.Settings.FirstOrDefault(x => x.Id == id);
        if (model is null)
            return;
        SelectedModel.Settings.Remove(model);

        var viewModel = Settings.FirstOrDefault(vm => vm.Model.Id == id);
        if (viewModel is null)
            return;
        Settings.Remove(viewModel);
    }

    private static class EventArgs
    {
        public static readonly PropertyChangedEventArgs SelectedModel = new(nameof(GraphGroupViewModel.SelectedModel));
        public static readonly PropertyChangedEventArgs GroupTitle = new(nameof(GraphGroupViewModel.GroupTitle));
        public static readonly PropertyChangedEventArgs IsWithLines = new(nameof(GraphGroupViewModel.IsWithLines));
        public static readonly PropertyChangedEventArgs LineWidthEnabled = new(nameof(GraphGroupViewModel.LineWidthEnabled));
        public static readonly PropertyChangedEventArgs LineWidthValue = new(nameof(GraphGroupViewModel.LineWidthValue));
        public static readonly PropertyChangedEventArgs LineWidthValueColor = new(nameof(GraphGroupViewModel.LineWidthValueColor));
        public static readonly PropertyChangedEventArgs LineTypeEnabled = new(nameof(GraphGroupViewModel.LineTypeEnabled));
        public static readonly PropertyChangedEventArgs LineTypeValue = new(nameof(GraphGroupViewModel.LineTypeValue));
        public static readonly PropertyChangedEventArgs LineTypeValueColor = new(nameof(GraphGroupViewModel.LineTypeValueColor));
        public static readonly PropertyChangedEventArgs IsWithPoints = new(nameof(GraphGroupViewModel.IsWithPoints));
        public static readonly PropertyChangedEventArgs PointsSizeEnabled = new(nameof(GraphGroupViewModel.PointsSizeEnabled));
        public static readonly PropertyChangedEventArgs PointsSizeValue = new(nameof(GraphGroupViewModel.PointsSizeValue));
        public static readonly PropertyChangedEventArgs PointsSizeValueColor = new(nameof(GraphGroupViewModel.PointsSizeValueColor));
        public static readonly PropertyChangedEventArgs PointsTypeEnabled = new(nameof(GraphGroupViewModel.PointsTypeEnabled));
        public static readonly PropertyChangedEventArgs PointsTypeValue = new(nameof(GraphGroupViewModel.PointsTypeValue));
        public static readonly PropertyChangedEventArgs PointsTypeValueColor = new(nameof(GraphGroupViewModel.PointsTypeValueColor));
    }
}
