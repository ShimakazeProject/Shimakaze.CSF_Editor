using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Microsoft.Win32;

using Shimakaze.ToolKit.CSF.GUI.ViewModel;
using Shimakaze.ToolKit.CSF.Kernel;

namespace Shimakaze.ToolKit.CSF.GUI.Data
{
    public static class DocumentManager
    {
        private static CsfBackgroundWorker<CsfClassFile> CsfClassFileBW;

        private const System.Windows.Visibility Hide = System.Windows.Visibility.Collapsed;
        private const System.Windows.Visibility Show = System.Windows.Visibility.Visible;
        public static void OpenFile(CsfDocumentView documentView, string filePath = null)

        {
            CsfClassFileBW = new CsfBackgroundWorker<CsfClassFile>();
            // 模式匹配

            // 显示状态块
            StatusBlock statusBlock = documentView.StatusBlock;
            statusBlock.Show(StatusBlock.Control.ProgressBar | StatusBlock.Control.TextBlock);
            statusBlock.ProgressBar.IsIndeterminate = true;

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
                    statusBlock.HideProgressBar();
                    return;
                }
            }
            // 设置取消按钮
            statusBlock.Text = "正在读取中, 请稍候...";
            statusBlock.ShowButton("取消");
            statusBlock.Button.Click += (o, e) =>
            {
                CsfClassFileBW.Cancel();
                statusBlock.Text = "用户已取消";
                statusBlock.HideProgressBar();
                statusBlock.HideButton();
            };
            // 属性变更通知
            CsfClassFileBW.PropertyChanged += (o, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(CsfClassFileBW.StatusString):// 字符串变更
                        lock (statusBlock)
                            statusBlock.Text = CsfClassFileBW.StatusString;
                        break;
                    case nameof(CsfClassFileBW.Error):// 发生异常
                        if (CsfClassFileBW.Error is Exception exception)
                        {
                            lock (statusBlock)
                                statusBlock.Text = exception.Message;
                            statusBlock.HideProgressBar();
                            statusBlock.HideButton();
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
            };

            // 加载文件
            CsfClassFileBW.BeginParse(new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read));
        }
    }
}
