using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows;

namespace Shimakaze.CSF.GUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private const string RegistryKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
        private const string RegistryValueName = "AppsUseLightTheme";

        private bool isDarkTheme = GetWindowsTheme();

        public bool IsDarkTheme
        {
            get => isDarkTheme; set
            {
                isDarkTheme = value;
                ColorThemeChange(value);
            }
        } 
        public static App Application { get; private set; } = Current as App;
        public App()
        {
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            ColorThemeChange(isDarkTheme);
            // now set the Green color scheme and dark base color
            //Fluent.ThemeManager.ChangeTheme(Application.Current, "Dark.Green");

            base.OnStartup(e);
        }
        private void ColorThemeChange(bool dark)
        {
            var paletteHelper = new PaletteHelper();
            var theme = paletteHelper.GetTheme();
            if (dark)
            {
                System.Diagnostics.Trace.WriteLine(string.Format("[{0}]\t暗色主题", nameof(ColorThemeChange)));
                Fluent.ThemeManager.ChangeThemeBaseColor(this, Fluent.ThemeManager.BaseColorDark);
                theme.SetBaseTheme(Theme.Dark);
            }
            else
            {
                System.Diagnostics.Trace.WriteLine(string.Format("[{0}]\t亮色主题", nameof(ColorThemeChange)));
                Fluent.ThemeManager.ChangeThemeBaseColor(this, Fluent.ThemeManager.BaseColorLight);
                theme.SetBaseTheme(Theme.Light);
            }
            paletteHelper.SetTheme(theme);
        }




        private static bool GetWindowsTheme()
        {
            using RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath);
            object registryValueObject = key?.GetValue(RegistryValueName);
            if (registryValueObject == null)
                return false;


            int registryValue = (int)registryValueObject;

            return registryValue > 0 ? false : true;
        }
    }
}