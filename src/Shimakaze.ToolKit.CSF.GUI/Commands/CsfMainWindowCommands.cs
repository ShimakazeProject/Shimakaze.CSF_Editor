using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Shimakaze.ToolKit.CSF.GUI.Commands
{
    public static class CsfMainWindowCommands
    {
        public static readonly RoutedUICommand OpenFile = new RoutedUICommand("OpenFile", "OpenFile", typeof(CsfMainWindowCommands));
        public static readonly RoutedUICommand SaveFile = new RoutedUICommand("SaveFile", "SaveFile", typeof(CsfMainWindowCommands));
        public static readonly RoutedUICommand SaveAsFile = new RoutedUICommand("SaveAsFile", "SaveAsFile", typeof(CsfMainWindowCommands));
        public static readonly RoutedUICommand NewFile = new RoutedUICommand("NewFile", "NewFile", typeof(CsfMainWindowCommands));

        public static readonly RoutedUICommand ImportFile = new RoutedUICommand("ImportFile", "ImportFile", typeof(CsfMainWindowCommands));
        public static readonly RoutedUICommand ExportFile = new RoutedUICommand("ExportFile", "ExportFile", typeof(CsfMainWindowCommands));
        public static readonly RoutedUICommand MergeFile = new RoutedUICommand("MergeFile", "MergeFile", typeof(CsfMainWindowCommands));

        public static readonly RoutedUICommand CloseFile = new RoutedUICommand("CloseFile", "CloseFile", typeof(CsfMainWindowCommands));

        public static readonly RoutedUICommand CloseWindow = new RoutedUICommand("CloseWindow", "CloseWindow", typeof(CsfMainWindowCommands));
    }
}
