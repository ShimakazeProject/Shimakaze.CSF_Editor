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
using Shimakaze.ToolKit.CSF.GUI.Theme;
using Shimakaze.ToolKit.CSF.GUI.String;

namespace Shimakaze.ToolKit.CSF.GUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public static App Instance { get; private set; } = Current as App;




        public App()
        {
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            this.AccentColorChange(ThemeManager.GetAccentColor());
            this.ColorThemeChange(ThemeManager.IsDarkTheme);

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


    }
}