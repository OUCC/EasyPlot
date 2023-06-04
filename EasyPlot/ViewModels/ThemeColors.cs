using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPlot.ViewModels
{
    public static class ThemeColors
    {
        public static Color TextBoxDisabled { get; } = Colors.WhiteSmoke;

        public static Color TextBoxEnabled { get; } = Colors.White;

        public static Color TabSelected { get; } = new Color(0xF0, 0xF0, 0xF0);

        public static Color TabNotSelected { get; } = Colors.LightGray;

        static ThemeColors()
        {
#if WINDOWS
            var uiSettings = new Windows.UI.ViewManagement.UISettings();
            var color = uiSettings.GetColorValue(Windows.UI.ViewManagement.UIColorType.Accent);
            Color.FromRgba(color.R, color.G, color.B, color.A);
#else
#endif
        }
    }
}
