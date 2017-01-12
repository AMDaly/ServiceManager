using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PeekServiceMonitor.ViewModel;
using System.Windows.Input;
using System.ServiceProcess;
using System.ComponentModel;
using System.Threading;

namespace PeekServiceMonitor.Commands
{
    class ServiceCommands : ICommand
    {
        private ServiceController svc;

        public void CommandStartService(ServiceController svc)
        {
            this.svc = svc;

            this.svc.Start();

            try
            {
                this.svc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(15));
            } catch (System.ServiceProcess.TimeoutException ex)
            {
                //log + display failure
            }

        }

        

        public void CommandRestartService(ServiceController svc)
        {
            this.svc = svc;

            this.svc.Stop();

            try
            {
                this.svc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(15));
            } catch (System.ServiceProcess.TimeoutException ex)
            {
                //log + display failure
            }
            

            this.svc.Start();

            try
            {
                this.svc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(15));
            } catch (System.ServiceProcess.TimeoutException ex)
            {
                //log + display failure
            }
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
            
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {

        }
    }
}
