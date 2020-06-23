using System.Windows.Input;

namespace Shimakaze.ToolKit.CSF.GUI.Commands
{
    public class CsfRibbonCommands
    {
        public static readonly RoutedUICommand NewClass = new RoutedUICommand("NewClass", "NewClass", typeof(CsfMainWindowCommands));
        public static readonly RoutedUICommand ReNameClass = new RoutedUICommand("ReNameClass", "ReNameClass", typeof(CsfMainWindowCommands));
        public static readonly RoutedUICommand ReMoveClass = new RoutedUICommand("ReMoveClass", "ReMoveClass", typeof(CsfMainWindowCommands));

        public static readonly RoutedUICommand NewLabel = new RoutedUICommand("NewLabel", "NewLabel", typeof(CsfMainWindowCommands));
        public static readonly RoutedUICommand EditLabel = new RoutedUICommand("EditLabel", "EditLabel", typeof(CsfMainWindowCommands));
        public static readonly RoutedUICommand ReMoveLabel = new RoutedUICommand("ReMoveLabel", "ReMoveLabel", typeof(CsfMainWindowCommands));


        public static readonly RoutedUICommand ToggleTheme = new RoutedUICommand("ToggleTheme", "ToggleTheme", typeof(CsfMainWindowCommands));
        public static readonly RoutedUICommand StartWindow = new RoutedUICommand("StartWindow", "StartWindow", typeof(CsfMainWindowCommands));
    }
}
