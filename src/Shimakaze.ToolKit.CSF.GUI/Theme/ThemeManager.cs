using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Interop;

using static MaterialDesignThemes.Wpf.ThemeExtensions;

namespace Shimakaze.ToolKit.CSF.GUI.Theme
{
    public static class ThemeManager
    {
        internal static void Initialize()
        {
            AppAccentColor = SystemAccentColor;
            AppBaseTheme = SystemBaseTheme;
        }

        private static bool isDarkBaseTheme;
        private static System.Windows.Media.Color appAccentColor;


        public static BaseTheme AppBaseTheme
        {
            get => isDarkBaseTheme ? BaseTheme.Dark : BaseTheme.Light; set
            {
                isDarkBaseTheme = value.Equals(BaseTheme.Dark);
                var paletteHelper = new MaterialDesignThemes.Wpf.PaletteHelper();
                var materialDesignTheme = paletteHelper.GetTheme();

                ControlzEx.Theming.ThemeManager.Current.ChangeThemeBaseColor(
                    App.Current,
                    isDarkBaseTheme
                    ? ControlzEx.Theming.ThemeManager.BaseColorDark
                    : ControlzEx.Theming.ThemeManager.BaseColorLight);
                materialDesignTheme.SetBaseTheme(
                    isDarkBaseTheme
                    ? MaterialDesignThemes.Wpf.Theme.Dark
                    : MaterialDesignThemes.Wpf.Theme.Light);

                paletteHelper.SetTheme(materialDesignTheme);
            }
        }
        internal static System.Windows.Media.Color AppAccentColor
        {
            get => appAccentColor; set
            {
                appAccentColor = value;
                var paletteHelper = new MaterialDesignThemes.Wpf.PaletteHelper();
                var materialDesignTheme = paletteHelper.GetTheme();
                var mahapps = ControlzEx.Theming.RuntimeThemeGenerator.Current.GenerateRuntimeTheme(
                    isDarkBaseTheme
                    ? ControlzEx.Theming.ThemeManager.BaseColorDark
                    : ControlzEx.Theming.ThemeManager.BaseColorLight, appAccentColor);
                ControlzEx.Theming.ThemeManager.Current.ClearThemes();
                ControlzEx.Theming.ThemeManager.Current.DetectTheme();
                ControlzEx.Theming.ThemeManager.Current.AddTheme(mahapps);
                ControlzEx.Theming.ThemeManager.Current.ChangeTheme(App.Current, mahapps);
                materialDesignTheme.SetPrimaryColor(appAccentColor);
                paletteHelper.SetTheme(materialDesignTheme);
            }
        }
        /// <summary>
        /// true 则 浅色 否则为false
        /// </summary>
        public static BaseTheme SystemBaseTheme
        {
            get
            {
                const string RegistryKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
                const string RegistryValueName = "AppsUseLightTheme";

                object result;
                if ((result = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(RegistryKeyPath)?.GetValue(RegistryValueName)) is null)
                    result = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(RegistryKeyPath)?.GetValue(RegistryValueName);

                if (result is int AppsUseLightTheme)
                    return AppsUseLightTheme > 0 ? BaseTheme.Light : BaseTheme.Dark;
                return BaseTheme.Light;
            }
        }
        /// <summary>
        /// 通过API获取系统主题色
        /// </summary>
        public static System.Windows.Media.Color SystemAccentColor
        {
            get
            {
                DwmGetColorizationColor(out var pcrColorization, out var pfOpaqueBlend);
                return new System.Windows.Media.Color()
                {
                    A = (byte)(pfOpaqueBlend ? 255 : pcrColorization >> 24),
                    R = (byte)(pcrColorization >> 16),
                    G = (byte)(pcrColorization >> 8),
                    B = (byte)(pcrColorization)
                };
            }
        }

        // See "https://docs.microsoft.com/en-us/windows/win32/api/dwmapi/nf-dwmapi-dwmgetcolorizationcolor"
        [System.Runtime.InteropServices.DllImport("dwmapi.dll", PreserveSig = false)]
        private static extern void DwmGetColorizationColor(out int pcrColorization, out bool pfOpaqueBlend);
        public enum BaseTheme : byte
        {
            Light, Dark
        }

        const int WM_DWMCOLORIZATIONCOLORCHANGED = 0x320;

        internal static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            //Debug.WriteLine($"[WndProc]捕获到消息:0x{msg.ToString("x").PadLeft(8, '0')}");
            switch (msg)
            {
                case WM_DWMCOLORIZATIONCOLORCHANGED:
                    /* 
                     * Update gradient brushes with new color information from
                     * NativeMethods.DwmGetColorizationParams() or the registry.
                     */

                    Initialize();

                    return IntPtr.Zero;

                default:
                    return IntPtr.Zero;
            }
        }
        internal static void Window_SourceInitialized(object sender, EventArgs e)
        {
            IntPtr hwnd;
            HwndSource hsource;
            if ((hwnd = new WindowInteropHelper(sender as Window).Handle) == IntPtr.Zero)
            {
                throw new InvalidOperationException("Could not get window handle.");
            }
            hsource = HwndSource.FromHwnd(hwnd);
            hsource.AddHook(WndProc);
        }
    }
}
