using System;
using System.ComponentModel;
using System.ServiceProcess;
using System.Windows.Input;

namespace PeekServiceMonitor.Commands
{
    public class RestartServiceCommand : ICommand
    {
        private ServiceController svc;

        public RestartServiceCommand(ServiceController svc)
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
            if (svc.Status == ServiceControllerStatus.Running && svc.CanStop)
            {
                return true;
            }

            return false;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            svc.Stop();

            try
            {
                svc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(15));
            }
            catch (System.ServiceProcess.TimeoutException ex)
            {
                //log + display failure
            }

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