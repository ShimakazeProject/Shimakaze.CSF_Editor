using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace Shimakaze.ToolKit.CSF.GUI
{
    /// <summary>
    /// StartScreen.xaml 的交互逻辑
    /// </summary>
    public partial class StartScreen
    {
        public void HidePub() => Hide();
        protected override async void Hide()
        {
            if (RootWindow is MainWindow) RootWindow.HideWaitScreen();
            while (true)
            {
                left.Opacity = right.Opacity -= 0.05;
                if (left.Opacity > 0) await Task.Delay(5);
                else break;
            }
            base.Hide();
            left.Visibility
                = right.Visibility
                = Visibility.Hidden; 
            left.Opacity = right.Opacity = 1;
        }
        public void ShowPub() => Show();
        protected override bool Show()
        {
            IsOpen = true;
            Shown = false;
            if (RootWindow is MainWindow) RootWindow.ShowWaitScreen();
            if(left != null)
            {
                left.Visibility
                    = right.Visibility
                    = Visibility.Visible;
            }
            return base.Show();
        }

        public StartScreen()
        {
            InitializeComponent();
            NothingLink.CommandParameter = EscKey.CommandParameter = this;
        }


        public MainWindow RootWindow
        {
            get { return (MainWindow)GetValue(RootWindowProperty); }
            set { SetValue(RootWindowProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RootWindow.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RootWindowProperty =
            DependencyProperty.Register("RootWindow", typeof(MainWindow), typeof(StartScreen), new PropertyMetadata(null));
    }
}
