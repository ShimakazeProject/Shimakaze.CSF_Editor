using MahApps.Metro;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace frg2089.CSF.Editor
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        //public App()
        //{
        //    bool darkMode = true;
        //    var res = new ResourceDictionary
        //    {
        //        Source = new Uri(darkMode ? "pack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseDark.xaml"
        //                                  : "pack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseLight.xaml")
        //    };
        //    this.Resources.MergedDictionaries.Add(res);
        //}
        protected override void OnStartup(StartupEventArgs e)
        {
            // add custom accent and theme resource dictionaries to the ThemeManager
            // you should replace MahAppsMetroThemesSample with your application name
            // and correct place where your custom accent lives
            ThemeManager.AddAccent("CustomAccent1", new Uri("pack://application:,,,/frg2089.CSF.Editor;component/Style.xaml"));


            // get the current app style (theme and accent) from the application
            // you can then use the current theme and custom accent instead set a new theme
            Tuple<AppTheme, Accent> appStyle = ThemeManager.DetectAppStyle(Application.Current);

            // now set the Green accent and dark theme
            ThemeManager.ChangeAppStyle(Application.Current,
                                        ThemeManager.GetAccent("CustomAccent1"),
                                        ThemeManager.GetAppTheme("BaseDark")); // or appStyle.Item1

            base.OnStartup(e);
        }
    }
}
