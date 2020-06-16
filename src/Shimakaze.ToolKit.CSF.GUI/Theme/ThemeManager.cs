using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using Microsoft.Win32;
using System.ComponentModel;
using System.Runtime.InteropServices;
using MaterialDesignThemes.Wpf;
using System.Windows.Media;
using System.Windows.Interop;
using System.Diagnostics;

namespace Shimakaze.ToolKit.CSF.GUI.Theme
{
    public static class ThemeManager
    {
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;

        private static bool isLightTheme = SystemUseLightTheme;

        public static bool IsLightTheme
        {
            get => isLightTheme; set
            {
                isLightTheme = value;
                App.Instance.ColorThemeChange(value);
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(IsLightTheme)));
            }
        }
        internal static void Initialize()
        {
            App.Instance.AccentColorChange(SystemAccentColor);
            App.Instance.ColorThemeChange(SystemUseLightTheme);
            //System.Windows.Interop.HwndSource.

        }

        internal static void ColorThemeChange(this App app, bool isLight)
        {


            var mahappsTheme = ControlzEx.Theming.ThemeManager.Current;
            var paletteHelper = new MaterialDesignThemes.Wpf.PaletteHelper();

            var materialDesignTheme = paletteHelper.GetTheme();
            var FluentAccentTheme = new System.Windows.ResourceDictionary();
            if (isLight)
            {
                System.Diagnostics.Trace.WriteLine(string.Format("[{0}]\t亮色主题", nameof(ColorThemeChange)));
                Fluent.ThemeManager.ChangeThemeBaseColor(app, Fluent.ThemeManager.BaseColorLight);
                mahappsTheme.ChangeThemeBaseColor(app, ControlzEx.Theming.ThemeManager.BaseColorLight);
                materialDesignTheme.SetBaseTheme(MaterialDesignThemes.Wpf.Theme.Light);
                FluentAccentTheme.Source = new Uri("/Theme/FluentTheme.Light.xaml", UriKind.Relative);
            }
            else
            {
                System.Diagnostics.Trace.WriteLine(string.Format("[{0}]\t暗色主题", nameof(ColorThemeChange)));
                Fluent.ThemeManager.ChangeThemeBaseColor(app, Fluent.ThemeManager.BaseColorDark);
                mahappsTheme.ChangeThemeBaseColor(app, ControlzEx.Theming.ThemeManager.BaseColorDark);
                materialDesignTheme.SetBaseTheme(MaterialDesignThemes.Wpf.Theme.Dark);
                FluentAccentTheme.Source = new Uri("/Theme/FluentTheme.Dark.xaml", UriKind.Relative);
            }
            paletteHelper.SetTheme(materialDesignTheme);
            app.Resources.MergedDictionaries.Add(FluentAccentTheme);
        }

        internal static void AccentColorChange(this App app, Color AccentBaseColor)
        {
            var paletteHelper = new MaterialDesignThemes.Wpf.PaletteHelper();
            var materialDesignTheme = paletteHelper.GetTheme();
            var mahapps = ControlzEx.Theming.RuntimeThemeGenerator.Current.GenerateRuntimeTheme(IsLightTheme ? "Dark" : "Light", AccentBaseColor);
            //var fluent = new Fluent.Theme(mahapps.Resources);
            // add custom theme resource dictionaries to the ThemeManager
            // you should replace FluentRibbonThemesSample with your application name
            // and correct place where your custom theme lives
            var resourceDictionary = new ResourceDictionary();
            resourceDictionary.Source = new Uri("/Theme/AccentTheme.xaml", UriKind.Relative);
            resourceDictionary.Remove("AccentBaseColor");
            resourceDictionary.Remove("AccentColor80");
            resourceDictionary.Remove("AccentColor60");
            resourceDictionary.Remove("AccentColor40");
            resourceDictionary.Remove("AccentColor20");
            resourceDictionary.Add("AccentBaseColor", mahapps.Resources["MahApps.Colors.AccentBase"]);
            resourceDictionary.Add("AccentColor80", mahapps.Resources["MahApps.Colors.Accent"]);
            resourceDictionary.Add("AccentColor60", mahapps.Resources["MahApps.Colors.Accent2"]);
            resourceDictionary.Add("AccentColor40", mahapps.Resources["MahApps.Colors.Accent3"]);
            resourceDictionary.Add("AccentColor20", mahapps.Resources["MahApps.Colors.Accent4"]);
            app.Resources.MergedDictionaries.Add(resourceDictionary);


            materialDesignTheme.SetPrimaryColor(AccentBaseColor);
            ControlzEx.Theming.ThemeManager.Current.ClearThemes();
            //Fluent.ThemeManager.ClearThemes();
            ControlzEx.Theming.ThemeManager.Current.DetectTheme();
            ControlzEx.Theming.ThemeManager.Current.AddTheme(mahapps);
            ControlzEx.Theming.ThemeManager.Current.ChangeTheme(app, mahapps);
            //Fluent.ThemeManager.ChangeTheme(app, fluent);
            paletteHelper.SetTheme(materialDesignTheme);



        }
        /// <summary>
        /// true 则 浅色 否则为false
        /// </summary>
        public static bool SystemUseLightTheme
        {
            get
            {
                const string RegistryKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
                const string RegistryValueName = "AppsUseLightTheme";

                object result;
                if ((result = Registry.CurrentUser.OpenSubKey(RegistryKeyPath)?.GetValue(RegistryValueName)) is null)
                    result = Registry.LocalMachine.OpenSubKey(RegistryKeyPath)?.GetValue(RegistryValueName);

                if (result is int AppsUseLightTheme)
                    return AppsUseLightTheme > 0;
                return true;
            }
        }
        /// <summary>
        /// 通过API获取系统主题色
        /// </summary>
        public static Color SystemAccentColor
        {
            get
            {
                DwmGetColorizationColor(out var pcrColorization, out var pfOpaqueBlend);
                return new Color()
                {
                    A = (byte)(pfOpaqueBlend ? 255 : pcrColorization >> 24),
                    R = (byte)(pcrColorization >> 16),
                    G = (byte)(pcrColorization >> 8),
                    B = (byte)(pcrColorization)
                };
            }
        }

        // See "https://docs.microsoft.com/en-us/windows/win32/api/dwmapi/nf-dwmapi-dwmgetcolorizationcolor"
        [DllImport("dwmapi.dll", PreserveSig = false)]
        private static extern void DwmGetColorizationColor(out int pcrColorization, out bool pfOpaqueBlend);

        const int WM_DWMCOLORIZATIONCOLORCHANGED = 0x320;

        internal static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            Debug.WriteLine($"[WndProc]捕获到消息:0x{msg.ToString("x").PadRight(8, '0')}");
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
