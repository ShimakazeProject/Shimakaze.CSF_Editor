using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;

namespace CSF.WPF.Core
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void About_Click(object sender, RoutedEventArgs e)
        {
            await this.ShowMessageAsync("关于此编辑器",string.Format("内部预览版 build:10654{0}Copyright © 2019 - 2020  舰队的偶像-岛风酱!", Environment.NewLine));
        }
    }
}
