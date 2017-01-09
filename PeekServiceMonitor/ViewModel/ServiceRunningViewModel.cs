using System;
using System.ServiceProcess;
using PeekServiceMonitor.PropertyChanged;
using log4net;
using log4net.Config;

namespace PeekServiceMonitor.ViewModel
{
    class ServiceRunningViewModel : NotifyPropertyChangedBase, IServiceRunningViewModel
    {
        private readonly ILog logger;
        private ServiceController svc;
        private readonly ServiceControllerStatus _originalServiceState;
        private ServiceControllerStatus _serviceState;

        public ServiceRunningViewModel(String svcName)
        {
            logger = LogManager.GetLogger(typeof(ServiceRunningViewModel));
            
            try
            {
                svc = new ServiceController(svcName);
                _originalServiceState = svc.Status;
            }
            catch (Exception ex)
            {
                logger.Warn(ex.StackTrace);
            }
        }
        
        public string Name
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

        public string Started
        {
            get { return "0"; }
        }

        public string Stopped
        {
            get { return "1"; }
        }

        public bool ChangesPending
        {
            get { return _serviceState != _originalServiceState; }
        }
    }
}
