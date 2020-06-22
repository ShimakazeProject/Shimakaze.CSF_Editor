using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// StatusBlock.xaml 的交互逻辑
    /// </summary>
    public partial class StatusBlock
    {
        public StatusBlock()
        {
            InitializeComponent();
        }
        public void ShowProgressBar() => ProgressBar.Visibility = Visibility.Visible;
        public void HideProgressBar() => ProgressBar.Visibility = Visibility.Collapsed;


        public void ShowProgressRing() => ProgressRing.Visibility = Visibility.Visible;
        public void HideProgressRing() => ProgressRing.Visibility = Visibility.Collapsed;

        public void ShowTextBlock() => TextBlock.Visibility = Visibility.Visible;
        public void HideTextBlock() => TextBlock.Visibility = Visibility.Collapsed;


        public void ShowButton(string s=null)
        {
            Button.Visibility = Visibility.Visible;
            if (!string.IsNullOrEmpty(s)) BtnContent = s;
        }

        public void HideButton() => Button.Visibility = Visibility.Collapsed;


        public void Show(Control control = Control.None)
        {
            Visibility = Visibility.Visible;
            if ((control & Control.ProgressBar) != Control.None) ShowProgressBar();
            if ((control & Control.ProgressRing) != Control.None) ShowProgressRing();
            if ((control & Control.TextBlock) != Control.None) ShowTextBlock();
            if ((control & Control.Button) != Control.None) ShowButton();
        }
        public void Hide() => Visibility = ProgressBar.Visibility = ProgressRing.Visibility = TextBlock.Visibility = Button.Visibility = Visibility.Collapsed;

        public string Text { get => TextBlock.Text; set => TextBlock.Text = value; }
        public object BtnContent { get => Button.Content; set => Button.Content = value; }

        [Flags]
        public enum Control
        {
            None = 0,
            ProgressBar = 1 << 0,
            ProgressRing = 1 << 1,
            TextBlock = 1 << 2,
            Button = 1 << 3,
        }
    }
}
