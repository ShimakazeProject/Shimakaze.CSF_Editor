using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Input;

using Microsoft.Win32;

using Shimakaze.ToolKit.CSF.GUI.Data;
using Shimakaze.ToolKit.CSF.GUI.ViewModel;
using Shimakaze.ToolKit.CSF.Kernel;

namespace Shimakaze.ToolKit.CSF.GUI.Commands
{
    public class OpenFileCommand : ICommand
    {
        public static OpenFileCommand Instance { get; } = new OpenFileCommand();
        public event EventHandler CanExecuteChanged;
        private ParseBackgroundWorker<CsfClassFile> CsfClassFileBW;

        public bool CanExecute(object parameter)
        {
            return true;
        }
        private const System.Windows.Visibility Hide = System.Windows.Visibility.Collapsed;
        private const System.Windows.Visibility Show = System.Windows.Visibility.Visible;
        public void Execute(object parameter)
        {
            CsfClassFileBW = new ParseBackgroundWorker<CsfClassFile>();
            // 模式匹配
            string filePath = null;
            CsfDocumentView documentView = parameter switch
            {
                MainWindow mainWindow => mainWindow.Document,
                StartScreen startScreen => startScreen.RootWindow.Document,
                CsfDocumentView csfDocumentView => csfDocumentView,
                null => throw new NotImplementedException(),
                _ => null,
            };
            (documentView, filePath) = parameter switch
            {
                ValueTuple<MainWindow, string> datatuple => (datatuple.Item1.Document, datatuple.Item2),
                ValueTuple<CsfDocumentView, string> datatuple => (datatuple.Item1, datatuple.Item2),
                ValueTuple<StartScreen, string> datatuple => (datatuple.Item1.RootWindow.Document, datatuple.Item2),
                _ => (documentView, null)
            };
            // 显示状态块
            StatusBlock statusBlock = documentView.StatusBlock;
            statusBlock.Visibility = Show;
            statusBlock.ProgressBar.IsIndeterminate = true;
            statusBlock.ProgressBar.Visibility = Show;

            if (string.IsNullOrEmpty(filePath))
            {
                // 创建打开文件对话框
                OpenFileDialog ofd = new OpenFileDialog
                {
                    Filter = "CSF 文件|*.csf"
                };
                //
                if (ofd.ShowDialog() ?? false) filePath = ofd.FileName;
                else
                {
                    statusBlock.ProgressBar.Visibility = Hide;
                    return;
                }
            }
            // 设置取消按钮
            statusBlock.TextBlock.Text = "正在读取中, 请稍候...";
            statusBlock.Button.Content = "取消";
            statusBlock.Button.Visibility = Show;
            statusBlock.Button.Click += (o, e) =>
            {
                CsfClassFileBW.Cancel();
                statusBlock.TextBlock.Text = "用户已取消";
                statusBlock.ProgressBar.Visibility = Hide;
                statusBlock.Button.Visibility = Hide;
            };
            // 属性变更通知
            CsfClassFileBW.PropertyChanged += (o, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(CsfClassFileBW.StatusString):// 字符串变更
                        lock (statusBlock.TextBlock)
                            statusBlock.TextBlock.Text = CsfClassFileBW.StatusString;
                        break;
                    case nameof(CsfClassFileBW.Error):// 发生异常
                        if (CsfClassFileBW.Error is Exception exception)
                        {
                            lock (statusBlock.TextBlock)
                                statusBlock.TextBlock.Text = exception.Message;
                            statusBlock.ProgressBar.Visibility = Hide;
                            //statusBlock.ProgressRing.Visibility = Hide;
                            statusBlock.Button.Visibility = Hide;
                        }
                        break;
                    case nameof(CsfClassFileBW.UnknowProgress):// 进度未知
                        if (CsfClassFileBW.UnknowProgress)
                        {
                            statusBlock.ProgressBar.IsIndeterminate = true;
                        }
                        else// 已知
                        {
                            statusBlock.ProgressBar.IsIndeterminate = false;
                            statusBlock.ProgressBar.Visibility = Show;
                            statusBlock.ProgressBar.Maximum = CsfClassFileBW.MaxProgress;
                        }
                        break;
                    case nameof(CsfClassFileBW.Progress):// 进度变更
                        lock (statusBlock.ProgressBar)
                            statusBlock.ProgressBar.Value = CsfClassFileBW.Progress;
                        break;
                }
            };
            // 任务完成
            CsfClassFileBW.Finished += (sender, result) =>
            {
                documentView.DataContext = new CsfDocument(result);
                if (result != null) statusBlock.Visibility = Hide;
                if (parameter is StartScreen startScreen) startScreen.HidePub();
            };

            // 加载文件
            CsfClassFileBW.BeginParse(new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read));
        }
    }
}
