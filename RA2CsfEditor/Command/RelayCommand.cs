using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RA2CsfEditor.Command
{
    /// <summary>
    /// 中继命令
    /// </summary>
    public class RelayCommand : ICommand
    {
        public RelayCommand(Action execute)
            : this(execute, null)
        {
        }

        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }

        //[DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute();
        }
        public event EventHandler CanExecuteChanged
        {   //这里把实现注释掉了，这样在SL下面也可以用。
            add { }
            remove { }
            //add
            //{
            //    if (_canExecute != null)
            //        CommandManager.RequerySuggested += value;
            //}
            //remove
            //{
            //    if (_canExecute != null)
            //        CommandManager.RequerySuggested -= value;
            //}
        }

        public void Execute(object parameter)
        {
            _execute();
        }

        readonly Action _execute;
        readonly Func<bool> _canExecute;
    }

    /// <summary>
    /// 中继命令 泛型
    /// </summary>
    /// <typeparam name="T">泛型</typeparam>
    public class RelayCommand<T> : CommandBase
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool> _canExecute;
        public RelayCommand(Action<T> execute)
            : this(execute, null)
        {
        }
        public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));

            if (canExecute == null)
                canExecute = _ => true;

            _execute = execute;
            _canExecute = canExecute;
        }

        public override bool CanExecute(object parameter)
        {
            return _canExecute((T)parameter);
        }

        protected override void OnExecute(object parameter)
        {
            _execute((T)parameter);
        }
    }

}
