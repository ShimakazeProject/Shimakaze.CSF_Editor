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
using System.Windows.Shapes;

using Microsoft.Win32;

using Shimakaze.ToolKit.CSF.GUI.Data;
using Shimakaze.ToolKit.CSF.GUI.ViewModel;
using Shimakaze.ToolKit.CSF.Kernel;

namespace Shimakaze.ToolKit.CSF.GUI
{
    /// <summary>
    /// OpenFileDialog.xaml 的交互逻辑
    /// </summary>
    public partial class OpenFileProgressDialog
    {
        private ParseBackgroundWorker<CsfClassFile> CsfClassFileBW;
        public event Action Finished;
        public OpenFileProgressDialog()
        {
            InitializeComponent();
            DataContext = CsfClassFileBW = new ParseBackgroundWorker<CsfClassFile>();
            CsfClassFileBW.Finished += CsfClassFileBW_Finished;
        }

        private void CsfClassFileBW_Finished(ParseBackgroundWorker<CsfClassFile> sender, CsfClassFile result)
        {
            var vm = new CsfDocument(result);
            Close();
            MainWindow.LastInstance.body.DataContext = vm;
            Finished?.Invoke();
        }


        public void StartTask()
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "CSF 文件|*.csf"
            };
            if (ofd.ShowDialog() ?? false)
            {
                var stream = ofd.OpenFile();
                CsfClassFileBW.BeginParse(stream);
              ShowDialog();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CsfClassFileBW.Cancel();
            Close();
        }
    }
}
