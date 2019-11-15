using System;
using System.Windows.Input;

namespace UniGraphics.ViewModels
{
    public class Command : ICommand
    {
        #region Constructor
        public Command(Action<object> action)
        {
            ExecuteDelegate = action;
        }
        #endregion
        #region Properties
        public Predicate<Object> CanExecuteDelegate { get; set; }
        public Action<Object> ExecuteDelegate { get; set; }
        #endregion
        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        public bool CanExecute(object parameter)
        {
            if (CanExecuteDelegate != null)
                return CanExecuteDelegate(parameter);
            return true;
        }

        public void Execute(object parameter)
        {
            if (ExecuteDelegate != null)
                ExecuteDelegate(parameter);
        }
    }
}
