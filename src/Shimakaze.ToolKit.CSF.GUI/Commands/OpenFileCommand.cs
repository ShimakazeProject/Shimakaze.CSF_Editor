using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

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
            var window = new OpenFileProgressDialog();
            if (parameter is Action act) window.Finished += act;
            window.StartTask();
        }
    }
}
