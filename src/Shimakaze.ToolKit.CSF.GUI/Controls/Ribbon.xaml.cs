using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Win32;

using Shimakaze.ToolKit.CSF.GUI.ViewModel;
using Shimakaze.ToolKit.CSF.Kernel;

namespace Shimakaze.ToolKit.CSF.GUI.Controls
{
    /// <summary>
    /// Ribbon.xaml 的交互逻辑
    /// </summary>
    public partial class Ribbon
    {
        public Ribbon()
        {
            InitializeComponent();
        }
        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            Theme.ThemeManager.IsDarkTheme = !Theme.ThemeManager.IsDarkTheme;
            if (Theme.ThemeManager.IsDarkTheme)
                (sender as Fluent.Button).Header = "LightTheme";
            else
                (sender as Fluent.Button).Header = "DarkTheme";

        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "CSF 文件|*.csf"
            };
            if (ofd.ShowDialog() ?? false)
            {
                var stream = ofd.OpenFile();
                var doc = new CsfClassFile();
                await doc.ParseAsync(stream);
                var vm = new CsfDocument(doc);
                MainWindow.LastInstance.body.DataContext = vm;
            }

        }
    }
}
