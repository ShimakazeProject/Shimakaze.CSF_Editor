using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Shimakaze.ToolKit.CSF.GUI.Commands
{
    public class RoutedCsfEditorCommands
    {
        public static readonly RoutedUICommand OpenFile = new RoutedUICommand("OpenFile", "OpenFile", typeof(RoutedCsfEditorCommands));
        public static readonly RoutedUICommand CloseFile = new RoutedUICommand("CloseFile", "CloseFile", typeof(RoutedCsfEditorCommands));
        public static readonly RoutedUICommand SaveFile = new RoutedUICommand("SaveFile", "SaveFile", typeof(RoutedCsfEditorCommands));
        public static readonly RoutedUICommand SaveAsFile = new RoutedUICommand("SaveAsFile", "SaveAsFile", typeof(RoutedCsfEditorCommands));
        public static readonly RoutedUICommand MergeFile = new RoutedUICommand("MergeFile", "MergeFile", typeof(RoutedCsfEditorCommands));
        public static readonly RoutedUICommand ImportFile = new RoutedUICommand("ImportFile", "ImportFile", typeof(RoutedCsfEditorCommands));
        public static readonly RoutedUICommand ExportFile = new RoutedUICommand("ExportFile", "ExportFile", typeof(RoutedCsfEditorCommands));
    }
}
