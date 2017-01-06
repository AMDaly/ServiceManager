using System;
using System.ServiceProcess;
using PeekServiceMonitor.PropertyChanged;

namespace PeekServiceMonitor.ViewModel
{
    class ServiceRunningViewModel : NotifyPropertyChangedBase, IServiceRunningViewModel
    {
        private ServiceController svc;
        private readonly ServiceControllerStatus _originalServiceState;
        private ServiceControllerStatus _serviceState;

        public ServiceRunningViewModel(String svcName)
        {
            try
            {
                svc = new ServiceController(svcName);
                var status = svc.Status;
            }
            catch
            {
                //log
            }
        }
        
        public String Name
        {
            get { return svc.DisplayName; }
        }

        public ServiceControllerStatus Status
        {
            get { return svc.Status; }
            set
            {
                if (SetField(ref _serviceState, value))
                {
                    OnPropertyChanged("ChangesPending");
                }
            }
        }

        public bool ChangesPending
        {
            get { return _serviceState != _originalServiceState; }
        }
    }
}
