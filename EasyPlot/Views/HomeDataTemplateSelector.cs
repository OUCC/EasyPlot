using EasyPlot.ViewModels.Wrapper;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace EasyPlot.Views;

public class HomeDataTemplateSelector : DataTemplateSelector
{
    public DataTemplate? Home { get; set; }

    public DataTemplate? GraphSettings { get; set; }

    protected override DataTemplate? SelectTemplateCore(object item)
    {
        if (item is GraphGroupViewModel viewModel)
        {
            return GraphSettings;
        }

        return Home;
    }
}
