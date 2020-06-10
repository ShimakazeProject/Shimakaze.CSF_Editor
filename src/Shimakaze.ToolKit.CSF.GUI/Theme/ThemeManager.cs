using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using Microsoft.Win32;
using System.ComponentModel;
using System.Runtime.InteropServices;
using MaterialDesignThemes.Wpf;

namespace Shimakaze.ToolKit.CSF.GUI.Theme
{
    public static class ThemeManager
    {
        private static bool isDarkTheme = GetWindowsTheme();

        public static bool IsDarkTheme
        {
            get => isDarkTheme; set
            {
                isDarkTheme = value;
                App.Instance.ColorThemeChange(value);
            }
        }
        private static bool GetWindowsTheme()
        {
            const string RegistryKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
            const string RegistryValueName = "AppsUseLightTheme";
            using RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath);
            object registryValueObject = key?.GetValue(RegistryValueName);
            if (registryValueObject == null)
                return false;


            int registryValue = (int)registryValueObject;

            return registryValue > 0 ? false : true;
        }
        public static System.Windows.Media.Color GetAccentColor()
        {
            //const string RegistryKeyPath = @"SOFTWARE\Microsoft\Windows\DWM";
            //const string RegistryValueName = "AccentColor";
            //using RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath);
            //object registryValueObject = key?.GetValue(RegistryValueName);
            //var color = System.Drawing.Color.FromArgb((Int32)registryValueObject);

            DwmGetColorizationParameters(out DWM_COLORIZATION_PARAMS dwmparams);
            var color = System.Drawing.Color.FromArgb((Int32)dwmparams.clrColor);


            return System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        internal static void ColorThemeChange(this App app, bool dark)
        {


            var mahappsTheme = ControlzEx.Theming.ThemeManager.Current;
            var paletteHelper = new MaterialDesignThemes.Wpf.PaletteHelper();

            var materialDesignTheme = paletteHelper.GetTheme();
            var FluentAccentTheme = new System.Windows.ResourceDictionary();
            if (dark)
            {
                System.Diagnostics.Trace.WriteLine(string.Format("[{0}]\t暗色主题", nameof(ColorThemeChange)));
                Fluent.ThemeManager.ChangeThemeBaseColor(app, Fluent.ThemeManager.BaseColorDark);
                mahappsTheme.ChangeThemeBaseColor(app, ControlzEx.Theming.ThemeManager.BaseColorDark);
                materialDesignTheme.SetBaseTheme(MaterialDesignThemes.Wpf.Theme.Dark);
                FluentAccentTheme.Source = new Uri("/Theme/FluentTheme.Dark.xaml", UriKind.Relative);
            }
            else
            {
                System.Diagnostics.Trace.WriteLine(string.Format("[{0}]\t亮色主题", nameof(ColorThemeChange)));
                Fluent.ThemeManager.ChangeThemeBaseColor(app, Fluent.ThemeManager.BaseColorLight);
                mahappsTheme.ChangeThemeBaseColor(app, ControlzEx.Theming.ThemeManager.BaseColorLight);
                materialDesignTheme.SetBaseTheme(MaterialDesignThemes.Wpf.Theme.Light);
                FluentAccentTheme.Source = new Uri("/Theme/FluentTheme.Light.xaml", UriKind.Relative);
            }
            paletteHelper.SetTheme(materialDesignTheme);
            app.Resources.MergedDictionaries.Add(FluentAccentTheme);
        }

        internal static void AccentColorChange(this App app, System.Windows.Media.Color AccentBaseColor)
        {
            var paletteHelper = new MaterialDesignThemes.Wpf.PaletteHelper();
            var materialDesignTheme = paletteHelper.GetTheme();
            var mahapps = ControlzEx.Theming.RuntimeThemeGenerator.Current.GenerateRuntimeTheme(IsDarkTheme ? "Dark" : "Light", AccentBaseColor);
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
            ControlzEx.Theming.ThemeManager.Current.ChangeTheme(app, mahapps);
            //Fluent.ThemeManager.ChangeTheme(app, fluent);
            paletteHelper.SetTheme(materialDesignTheme);



        }

        [DllImport("dwmapi.dll", EntryPoint = "#127", PreserveSig = false)]
        private static extern void DwmGetColorizationParameters(out DWM_COLORIZATION_PARAMS parameters);

        private struct DWM_COLORIZATION_PARAMS
        {
            public uint clrColor;
            public uint clrAfterGlow;
            public uint nIntensity;
            public uint clrAfterGlowBalance;
            public uint clrBlurBalance;
            public uint clrGlassReflectionIntensity;
            public bool fOpaque;
        }
    }
}
