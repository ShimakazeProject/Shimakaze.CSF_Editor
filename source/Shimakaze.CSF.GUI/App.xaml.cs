using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows;
using Shimakaze.CSF.GUI.Theme;

namespace Shimakaze.CSF.GUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {

        private bool isDarkTheme = GetWindowsTheme();

        public bool IsDarkTheme
        {
            get => isDarkTheme; set
            {
                isDarkTheme = value;
                this.ColorThemeChange(value);
            }
        } 
        public static App Application { get; private set; } = Current as App;
        public App()
        {
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            this.AccentColorChange(GetAccentColor());
            this.ColorThemeChange(isDarkTheme);

            base.OnStartup(e);
            int timer = 3000;

            //initialize the splash screen and set it as the application main window
            var splashScreen = new SplashWindow();
            this.MainWindow = splashScreen;
            splashScreen.Show();

            //in order to ensure the UI stays responsive, we need to
            //do the work on a different thread
            Task.Factory.StartNew(() =>
            {
                //simulate some work being done
                System.Threading.Thread.Sleep(timer);

                //since we're not on the UI thread
                //once we're done we need to use the Dispatcher
                //to create and show the main window
                this.Dispatcher.Invoke(() =>
                {
                    //initialize the main window, set it as the application main window
                    //and close the splash screen
                    var mainWindow = new MainWindow();
                    this.MainWindow = mainWindow;
                    mainWindow.Show();
                    splashScreen.Close();
                });
            });
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
        private static System.Windows.Media.Color GetAccentColor()
        {
            const string RegistryKeyPath = @"SOFTWARE\Microsoft\Windows\DWM";
            const string RegistryValueName = "AccentColor";
            using RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath);
            object registryValueObject = key?.GetValue(RegistryValueName);
            var color = System.Drawing.Color.FromArgb((Int32)registryValueObject);


            return System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
        }
    }
}