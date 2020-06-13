using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Windows.Input;

using Shimakaze.ToolKit.CSF.GUI.ViewModel;

namespace Shimakaze.ToolKit.CSF.GUI.Commands
{
    public class OpenFileCommand : ICommand
    {
        public static OpenFileCommand Instance { get; } = new OpenFileCommand();
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var openTask = new OpenFileProgressDialog();
            switch (parameter)
            {
                case MainWindow mainWindow:
                    openTask.CsfClassFileBW.Finished += (sender, result) =>
                        mainWindow.Document.DataContext = new CsfDocument(result);
                    break;
                case CsfDataGrid documentGrid:
                    openTask.CsfClassFileBW.Finished += (sender, result) =>
                        documentGrid.DataContext = new CsfDocument(result);
                    break;
            }
            openTask.StartTask();
        }
    }
}
