using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
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
            SourceInitialized += Theme.ThemeManager.Window_SourceInitialized;
        }
        public MainWindow(string filePath) : this()
        {
            Ribbon.startScreen.IsOpen = false;
            HideWaitScreen();
            Document.OpenFile(filePath);
        }
        public void ShowWaitScreen()
        {
            StatusBlock.ProgressRing.IsActive = true;
            StatusBlock.Show(StatusBlock.Control.TextBlock | StatusBlock.Control.ProgressRing, "正在准备中...");
        }
        public void HideWaitScreen() => StatusBlock.Hide();

        private void OpenFile_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void OpenFile_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            StatusBlock statusBlock = Document.StatusBlock;
            statusBlock.ProgressBar.IsIndeterminate = true;
            statusBlock.Show(StatusBlock.Control.ProgressBar | StatusBlock.Control.TextBlock, "正在等待...");
            // 创建打开文件对话框
            var ofd = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "CSF 文件|*.csf"
            };
            if (ofd.ShowDialog() ?? false) Document.OpenFile(ofd.FileName);
            else
            {
                statusBlock.TextBlock.Text = "已取消";
                statusBlock.HideProgressBar();
                return;
            }

        }

        private void SaveFile_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Document.IsOpenedFile;
        }

        private void SaveFile_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            Document.SaveFile(Document.FilePath);
        }

        private void SaveAsFile_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Document.IsOpenedFile;
        }

        private void SaveAsFile_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            StatusBlock statusBlock = Document.StatusBlock;
            statusBlock.ProgressBar.IsIndeterminate = true;
            statusBlock.Show(StatusBlock.Control.ProgressBar | StatusBlock.Control.TextBlock, "正在等待...");
            // 创建保存文件对话框
            var ofd = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "CSF 文件|*.csf",
                FileName = "ra2md.csf"
            };
            if (ofd.ShowDialog() ?? false) Document.SaveFile(ofd.FileName);
            else
            {
                statusBlock.TextBlock.Text = "已取消";
                statusBlock.HideProgressBar();
                return;
            }
        }

        private void NewFile_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void NewFile_Execute(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void ImportFile_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ImportFile_Execute(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void ExportFile_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ExportFile_Execute(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void MergeFile_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void MergeFile_Execute(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void CloseFile_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CloseFile_Execute(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void CloseWindow_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CloseWindow_Execute(object sender, ExecutedRoutedEventArgs e)
        {

        }
    }
}