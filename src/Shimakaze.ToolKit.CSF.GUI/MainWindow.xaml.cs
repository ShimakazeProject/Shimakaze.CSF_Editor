using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Fluent;
using MahApps.Metro.Controls;

namespace Shimakaze.ToolKit.CSF.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow 
    {
        public MainWindow()
        {
            InitializeComponent();
            Ribbon.RootWindow = this;
            ShowWaitScreen();
        }
        public MainWindow(string filePath) : this()
        {
            Ribbon.StartScreen.IsOpen = false;
            HideWaitScreen();
            Commands.OpenFileCommand.Instance.Execute((this, filePath));
        }
        public void ShowWaitScreen()
        {
            StatusBlock.Button.Visibility
                = StatusBlock.ProgressBar.Visibility
                = Visibility.Collapsed;
            StatusBlock.ProgressRing.IsActive = true;
            StatusBlock.ProgressRing.Visibility = Visibility.Visible;
            StatusBlock.TextBlock.Text = "正在准备中...";
            StatusBlock.Visibility = Visibility.Visible;
        }
        public void HideWaitScreen()
        {
            StatusBlock.Visibility = Visibility.Collapsed;
        }
    }
}