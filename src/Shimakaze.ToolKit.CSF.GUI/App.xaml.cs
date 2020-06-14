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
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        private async void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            using var fs = new System.IO.FileStream($"{DateTime.Now:yyyy-MM-dd_HH+mm+ss}.log", System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write, System.IO.FileShare.Read);
            using var sw = new System.IO.StreamWriter(fs);
            await sw.WriteLineAsync(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ms"));
            await sw.WriteLineAsync(e.Exception.ToString());
            MessageBox.Show($"异常信息: {e.Exception}", "内部错误");
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

            var args = Environment.GetCommandLineArgs();
            MainWindow mainWindow = null;
            for (int i = 1; i < args.Length; i++)
            {
                if (args[i].Equals("-o") && args[i].Equals("--open"))
                {
                    i++;
                    mainWindow = new MainWindow(args[i]);
                }
            }
            if (mainWindow is null) mainWindow = new MainWindow();

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

                    this.MainWindow = mainWindow;
                    mainWindow.Show();
                    splashScreen.Close();
                });
            });
        }


    }
}