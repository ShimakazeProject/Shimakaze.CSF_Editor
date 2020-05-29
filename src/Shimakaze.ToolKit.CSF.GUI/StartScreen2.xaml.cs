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

namespace Shimakaze.ToolKit.CSF.GUI
{
    /// <summary>
    /// StartScreen.xaml 的交互逻辑
    /// </summary>
    public partial class StartScreen2 : UserControl
    {
        public StartScreen2()
        {
            InitializeComponent();
        }
        public void Show()
        {
            this.Visibility = Visibility.Visible;
        }
        public void Hide()
        {
            this.Visibility = Visibility.Collapsed;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }
    }
}
