using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using PeekServiceMonitor.ViewModel;

namespace PeekServiceMonitor.Commands
{
    public class StopServiceCommand : ICommand
    {
        private ServiceController svc;

        public StopServiceCommand(ServiceController svc)
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
            try
            {
                svc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(15));
            }
            catch (System.ServiceProcess.TimeoutException ex)
            {
                //log + display failure
            }
        }
    }
}
