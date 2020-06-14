using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Shimakaze.ToolKit.CSF.GUI.Commands
{
    public class CloseWindowCommand : ICommand
    {
        public static CloseWindowCommand Instance { get; } = new CloseWindowCommand();
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            // TODO: 获取到DocumentView
            // TODO: 检查文件是否被修改后没有保存 然后弹出保存对话框
            // 
            switch (parameter)
            {
                case Window window:
                    window.Close();
                    break;
                case Application app:
                    app.Shutdown();
                    break;
                case null:
                    Environment.Exit(0);
                    break;
            }
        }
    }
}
