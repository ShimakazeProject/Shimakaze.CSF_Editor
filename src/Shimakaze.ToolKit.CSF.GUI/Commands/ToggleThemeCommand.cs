using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Shimakaze.ToolKit.CSF.GUI.Commands
{
    public class ToggleThemeCommand : ICommand
    {
        public static ToggleThemeCommand Instance { get; } = new ToggleThemeCommand();
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Theme.ThemeManager.IsLightTheme = !Theme.ThemeManager.IsLightTheme;
        }
    }
}
