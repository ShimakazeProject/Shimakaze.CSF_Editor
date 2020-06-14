using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Shimakaze.ToolKit.CSF.GUI.Commands
{
    public class ShowStartScreenCommand : ICommand
    {
        public static ShowStartScreenCommand Instance { get; } = new ShowStartScreenCommand();
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            switch (parameter)
            {
                case Ribbon ribbon:
                    ribbon.StartScreen.ShowPub();
                    break;
                case StartScreen startScreen:
                    startScreen.ShowPub();
                    break;
            }
        }
    }
}
