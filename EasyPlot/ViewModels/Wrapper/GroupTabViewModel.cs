using System.ComponentModel;
using EasyPlot.Models;

namespace EasyPlot.ViewModels.Wrapper
{
    internal class GroupTabViewModel : INotifyPropertyChanged, IDisposable
    {
        public GroupTabViewModel(GraphGroupModel model, GraphGroupViewModel selectedGroup)
        {
            _model = model;
            _selectedGroup = selectedGroup;

            _selectedGroup.PropertyChanged += OnSelectedGroupPropertyChanged;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly GraphGroupModel _model;

        private readonly GraphGroupViewModel _selectedGroup;

        private bool _disposed;

        public int Id => _model.Id;

        public string GroupTitle => _model.GroupTitle;

        public bool Selected
        {
            get => _selected;
            set
            {
                if (_selected != value)
                {
                    _selected = value;
                    PropertyChanged?.Invoke(this, new(nameof(BackgroundColor)));
                }
            }
        }
        private bool _selected = false;

        public Color BackgroundColor => Selected ? ThemeColors.TabSelected : ThemeColors.TabNotSelected;

        private void OnSelectedGroupPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (PropertyChanged is null)
                return;

            if (e.PropertyName == nameof(GraphGroupViewModel.GroupTitle))
            {
                PropertyChanged(this, new(nameof(GroupTitle)));
            }
        }

        public void Dispose()
        {
            if (_disposed) return;

            _selectedGroup.PropertyChanged -= OnSelectedGroupPropertyChanged;

            _disposed = true;
        }
    }
}
