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
    /// CsfDocumentView.xaml 的交互逻辑
    /// </summary>
    public partial class CsfDocumentView
    {
        public CsfDocumentView()
        {
            InitializeComponent();
            StatusBlock.ProgressBar.Visibility
                = StatusBlock.ProgressRing.Visibility
                = StatusBlock.Button.Visibility
                = Visibility.Collapsed;
            StatusBlock.TextBlock.Text = "请拖放或打开一个文件以继续";
        }

        private void OpenFile_Drop(object sender, DragEventArgs e)
        {
            Commands.OpenFileCommand.Instance.Execute((this, ((Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString()));
        }
    }
}
