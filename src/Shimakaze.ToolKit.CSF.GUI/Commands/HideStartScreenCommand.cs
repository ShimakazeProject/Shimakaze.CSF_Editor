using System;
using System.Windows.Input;

namespace Shimakaze.ToolKit.CSF.GUI.Commands
{
    public class HideStartScreenCommand : ICommand
    {
        public static HideStartScreenCommand Instance { get; } = new HideStartScreenCommand();
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
                    ribbon.startScreen .HidePub();
                    break;
                case StartScreen startScreen:
                    startScreen.HidePub();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
