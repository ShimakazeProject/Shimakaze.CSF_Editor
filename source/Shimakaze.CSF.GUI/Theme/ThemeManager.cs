using static MaterialDesignThemes.Wpf.ThemeExtensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Shimakaze.CSF.GUI.Theme
{
    public static class ThemeManager
    {
        internal static void ColorThemeChange(this App app,bool dark)
        {
            var paletteHelper = new MaterialDesignThemes.Wpf.PaletteHelper();
            var theme = paletteHelper.GetTheme();
            var FluentAccentTheme = new System.Windows.ResourceDictionary();
            if (dark)
            {
                System.Diagnostics.Trace.WriteLine(string.Format("[{0}]\t暗色主题", nameof(ColorThemeChange)));
                Fluent.ThemeManager.ChangeThemeBaseColor(app, Fluent.ThemeManager.BaseColorDark);
                theme.SetBaseTheme(MaterialDesignThemes.Wpf.Theme.Dark);
                FluentAccentTheme.Source = new Uri("/Theme/FluentTheme.Dark.xaml", UriKind.Relative);
            }
            else
            {
                System.Diagnostics.Trace.WriteLine(string.Format("[{0}]\t亮色主题", nameof(ColorThemeChange)));
                Fluent.ThemeManager.ChangeThemeBaseColor(app, Fluent.ThemeManager.BaseColorLight);
                theme.SetBaseTheme(MaterialDesignThemes.Wpf.Theme.Light);
                FluentAccentTheme.Source = new Uri("/Theme/FluentTheme.Light.xaml", UriKind.Relative);
            }
            paletteHelper.SetTheme(theme);
            app.Resources.MergedDictionaries.Add(FluentAccentTheme);
        }

        internal static void AccentColorChange(this App app,System.Windows.Media.Color AccentBaseColor)
        {
            System.Windows.Media.Color AccentColor80, AccentColor60, AccentColor40, AccentColor20;
            AccentColor80 = System.Windows.Media.Color.FromArgb(0x80, AccentBaseColor.R, AccentBaseColor.G, AccentBaseColor.B);
            AccentColor60 = System.Windows.Media.Color.FromArgb(0x60, AccentBaseColor.R, AccentBaseColor.G, AccentBaseColor.B);
            AccentColor40 = System.Windows.Media.Color.FromArgb(0x40, AccentBaseColor.R, AccentBaseColor.G, AccentBaseColor.B);
            AccentColor20 = System.Windows.Media.Color.FromArgb(0x20, AccentBaseColor.R, AccentBaseColor.G, AccentBaseColor.B);
            // add custom theme resource dictionaries to the ThemeManager
            // you should replace FluentRibbonThemesSample with your application name
            // and correct place where your custom theme lives
            var resourceDictionary = new ResourceDictionary();
            resourceDictionary.Source = new Uri("/Theme/AccentTheme.xaml",UriKind.Relative);
            resourceDictionary.Remove("AccentBaseColor");
            resourceDictionary.Remove("AccentColor80");
            resourceDictionary.Remove("AccentColor60");
            resourceDictionary.Remove("AccentColor40");
            resourceDictionary.Remove("AccentColor20");
            resourceDictionary.Add("AccentBaseColor", AccentBaseColor);
            resourceDictionary.Add("AccentColor80", AccentColor80);
            resourceDictionary.Add("AccentColor60", AccentColor60);
            resourceDictionary.Add("AccentColor40", AccentColor40);
            resourceDictionary.Add("AccentColor20", AccentColor20);
            app.Resources.MergedDictionaries.Add(resourceDictionary);


            //Fluent.ThemeManager.AddTheme(FluentAccentTheme);
            //// get the current theme from the application
            //var theme = Fluent.ThemeManager.DetectTheme(this);
            //// now change theme to the custom theme
            //Fluent.ThemeManager.ChangeTheme(this, Fluent.ThemeManager.GetTheme(FluentAccentTheme));

        }
    }
}
