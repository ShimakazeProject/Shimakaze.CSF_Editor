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
            Theme.ThemeManager.AppBaseTheme =
                Theme.ThemeManager.AppBaseTheme.Equals(Theme.ThemeManager.BaseTheme.Dark)
                    ? Theme.ThemeManager.BaseTheme.Light
                    : Theme.ThemeManager.BaseTheme.Dark;
        }
    }
}
