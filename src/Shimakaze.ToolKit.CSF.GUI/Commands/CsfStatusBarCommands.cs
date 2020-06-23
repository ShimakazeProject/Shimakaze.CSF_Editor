using System.Windows.Input;

namespace Shimakaze.ToolKit.CSF.GUI.Commands
{
    public class CsfStatusBarCommands
    {
        public static readonly RoutedUICommand EditVersion = new RoutedUICommand("EditVersion", "EditVersion", typeof(CsfStatusBarCommands));
        public static readonly RoutedUICommand EditLanguage = new RoutedUICommand("EditLanguage", "EditLanguage", typeof(CsfStatusBarCommands));
    }
}
