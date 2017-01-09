using System;
using System.Threading;
using System.Windows.Input;
using log4net;
using log4net.Config;
using PeekServiceMonitor.Wpf;

namespace PeekServiceMonitor.Commands
{
    public abstract class WpfCommandBase<T> : ICommand
    {
        private int _currentlyExecuting;
        private readonly ILog logger;

        public bool CanExecute(object parameter)
        {
            try
            {
                return CanExecute((T)parameter);
            }
            catch (InvalidCastException e)
            {
                this.logger.Error(string.Format("Cannot cast parameter ({0}) to {1}.", parameter == null ? "[null]" : parameter.GetType().Name, typeof(T).Name), e);
            }

            return false;
        }

        public bool CanExecute(T parameter)
        {
            var canExecute = (AllowMultipleExecution || _currentlyExecuting == 0) && CanExecuteInternal(parameter);
            return canExecute;
        }

        protected virtual bool CanExecuteInternal(T parameter)
        {
            // Default operation is to always return true.
            return true;
        }

        public void Execute(object parameter)
        {
            try
            {
                Execute((T)parameter);
            }
            catch (InvalidCastException e)
            {
                this.logger.Error(string.Format("Cannot cast parameter ({0}) to {1}.", parameter == null ? "[null]" : parameter.GetType().Name, typeof(T).Name), e);
                throw;
            }
        }

        public void Execute(T parameter)
        {
            RegisterExecutionStarted();

            try
            {
                ExecuteInternal(parameter);
            }
            finally
            {
                RegisterExecutionCompleted();
            }
        }

        protected abstract void ExecuteInternal(T parameter);

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Indicates if the command can be executed multiple times in parallel.
        /// </summary>
        public bool AllowMultipleExecution { get; set; }

        /// <summary>
        /// Indicates if the command is currently executing.
        /// </summary>
        public bool CurrentlyExecuting { get { return _currentlyExecuting > 0; } }

        /// <summary>
        /// Registers that the command execution (or a part of it) has started. This can be called multiple times (e.g. from different threads), but
        /// each call must have a matching call to <see cref="RegisterExecutionCompleted"/>.
        /// </summary>
        public void RegisterExecutionStarted()
        {
            var executionCount = Interlocked.Increment(ref _currentlyExecuting);
            if (executionCount == 1) WpfHelper.TriggerInvalidateRequerySuggestedOnUiThread();
        }

        /// <summary>
        /// Registers that the command execution (or a part of it) has completed. This can be called multiple times (e.g. from different threads), but
        /// must appear after the matching call to <see cref="RegisterExecutionStarted"/>.
        /// </summary>
        public void RegisterExecutionCompleted()
        {
            var executionCount = Interlocked.Decrement(ref _currentlyExecuting);
            if (executionCount == 0) WpfHelper.TriggerInvalidateRequerySuggestedOnUiThread();

            if (_currentlyExecuting < 0) throw new InvalidOperationException("RegisterExecutionCompleted called without previous call to RegisterExecutionStarted");
        }
    }
}
