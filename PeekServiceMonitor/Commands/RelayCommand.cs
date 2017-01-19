using System;
using System.Diagnostics;
using System.Windows.Input;

namespace PeekServiceMonitor.Commands
{
    /// <summary>
    /// Code stolen from the MSDN article "WPF Apps With The Model-View-ViewModel Design Pattern" by Josh Smith.
    /// Original article located at: http://msdn.microsoft.com/en-us/magazine/dd419663.aspx
    /// </summary>
    /// <typeparam name="T">The type of object which will be passed into the <see cref="ICommand.CanExecute"/> and <see cref="ICommand.Execute"/> methods</typeparam>
    public class RelayCommand<T> : ICommand
    {
        #region Fields

        readonly Action<T> _execute;
        readonly Predicate<T> _canExecute;
        private Predicate<T> _wrappedCanExecute;
        private bool _currentlyExecuting;

        #endregion // Fields

        #region Constructors

        public RelayCommand(Action<T> execute, Predicate<T> canExecute = null)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute ?? (o => true);
            AllowMultipleExecution = false;
        }

        #endregion // Constructors

        public bool AllowMultipleExecution
        {
            get { return _wrappedCanExecute == _canExecute; }
            set
            {
                if (!value)
                {
                    _wrappedCanExecute = o => !_currentlyExecuting && _canExecute(o);
                }
                else
                {
                    _wrappedCanExecute = _canExecute;
                }
            }
        }

        #region ICommand Members

        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return _wrappedCanExecute((T)parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            _currentlyExecuting = true;

            try
            {
                _execute((T)parameter);
            }
            finally
            {
                _currentlyExecuting = false;
            }
        }

        #endregion // ICommand Members
    }

    /// <summary>
    /// Non generic variant.
    /// </summary>
    public class RelayCommand : RelayCommand<object>
    {
        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null) : base(execute, canExecute)
        {
        }
    }
}