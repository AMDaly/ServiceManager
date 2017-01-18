using System;
using System.ComponentModel;
using System.ServiceProcess;
using System.Windows.Input;

namespace PeekServiceMonitor.Commands
{
    public class StartServiceCommand : ICommand
    {
        private ServiceController svc;

        public StartServiceCommand(ServiceController svc)
        {
            this.svc = svc;
        }

        private void svc_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }

        public bool CanExecute(object parameter)
        {
            if (svc.Status == ServiceControllerStatus.Stopped)
            {
                return true;
            }

            return false;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            svc.Start();

            try
            {
                svc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(15));
            }
            catch (System.ServiceProcess.TimeoutException ex)
            {
                //log + display failure
            }
        }
    }
}