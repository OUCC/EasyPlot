// Author: LanceMcCarthy
// https://gist.github.com/LanceMcCarthy/4954ab92ca44c19eb4316d9d683efd50

using Microsoft.UI.Composition;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using WinRT;

namespace OUCC.EasyPlot.WinUI;

public static class WindowHelpers
{
    public static void TryMicaOrAcrylic(this Microsoft.UI.Xaml.Window window)
    {
        var dispatcherQueueHelper = new WindowsSystemDispatcherQueueHelper(); // in Platforms.Windows folder
        dispatcherQueueHelper.EnsureWindowsSystemDispatcherQueueController();

        // Hooking up the policy object
        var configurationSource = new SystemBackdropConfiguration
        {
            IsInputActive = true
        };

        ((FrameworkElement)window.Content).ActualThemeChanged += (sender, args) =>
        {
            configurationSource.Theme = sender.ActualTheme switch
            {
                ElementTheme.Dark => SystemBackdropTheme.Dark,
                ElementTheme.Light => SystemBackdropTheme.Light,
                _ => SystemBackdropTheme.Default
            };
        };

        configurationSource.Theme = ((FrameworkElement)window.Content).ActualTheme switch
        {
            ElementTheme.Dark => SystemBackdropTheme.Dark,
            ElementTheme.Light => SystemBackdropTheme.Light,
            _ => SystemBackdropTheme.Default
        };

        // Let's try Mica first
        if (MicaController.IsSupported())
        {
            var micaController = new MicaController();
            micaController.AddSystemBackdropTarget(window.As<ICompositionSupportsSystemBackdrop>());
            micaController.SetSystemBackdropConfiguration(configurationSource);

            window.Activated += (object sender, WindowActivatedEventArgs args) =>
            {
                if (args.WindowActivationState is WindowActivationState.CodeActivated or WindowActivationState.PointerActivated)
                {
                    // Handle situation where a window is activated and placed on top of other active windows.
                    if (micaController == null)
                    {
                        micaController = new MicaController();
                        micaController.AddSystemBackdropTarget(window.As<ICompositionSupportsSystemBackdrop>()); // Requires 'using WinRT;'
                        micaController.SetSystemBackdropConfiguration(configurationSource);
                    }
                    if (configurationSource != null)
                        configurationSource.IsInputActive = args.WindowActivationState != WindowActivationState.Deactivated;
                }
            };

            window.Closed += (object sender, WindowEventArgs args) =>
            {
                if (micaController != null)
                {
                    micaController.Dispose();
                    micaController = null;
                }
                configurationSource = null;
            };
        }
        // If no Mica, maybe we can use Acrylic instead
        else if (DesktopAcrylicController.IsSupported())
        {
            var acrylicController = new DesktopAcrylicController();
            acrylicController.AddSystemBackdropTarget(window.As<ICompositionSupportsSystemBackdrop>());
            acrylicController.SetSystemBackdropConfiguration(configurationSource);

            window.Activated += (object sender, WindowActivatedEventArgs args) =>
            {
                if (args.WindowActivationState is WindowActivationState.CodeActivated or WindowActivationState.PointerActivated)
                {
                    // Handle situation where a window is activated and placed on top of other active windows.
                    if (acrylicController == null)
                    {
                        acrylicController = new DesktopAcrylicController();
                        acrylicController.AddSystemBackdropTarget(window.As<ICompositionSupportsSystemBackdrop>()); // Requires 'using WinRT;'
                        acrylicController.SetSystemBackdropConfiguration(configurationSource);
                    }
                }
                if (configurationSource != null)
                    configurationSource.IsInputActive = args.WindowActivationState != WindowActivationState.Deactivated;
            };

            window.Closed += (object sender, WindowEventArgs args) =>
            {
                if (acrylicController != null)
                {
                    acrylicController.Dispose();
                    acrylicController = null;
                }
                configurationSource = null;
            };
        }
    }
}
