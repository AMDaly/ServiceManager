using System;
using System.Diagnostics;
using System.ServiceProcess;
using PeekServiceMonitor.PropertyChanged;
using log4net;
using PeekServiceMonitor.Commands;
using PeekServiceMonitor.Wpf;
using System.ComponentModel;

namespace PeekServiceMonitor.ViewModel
{
    public class ServiceRunningViewModel : NotifyPropertyChangedBase, IServiceRunningViewModel
    {
        private readonly ILog logger;
        private ServiceController svc;
        private readonly ServiceControllerStatus _originalServiceState;
        private ServiceControllerStatus _serviceState;
        private Process process;
        private int id;
        private string startTime;
        private string uptime;
        private bool _selected;

        public ServiceRunningViewModel(String svcName)
        {
            logger = LogManager.GetLogger(typeof(ServiceRunningViewModel));
            ProcessExtensions processExtensions = new ProcessExtensions();

            try
            {
                svc = new ServiceController(svcName);
                _originalServiceState = svc.Status;
                id = processExtensions.GetProcessId(svc);
                startTime = processExtensions.GetStartTime(id);

                if (svc.Status == ServiceControllerStatus.Running)
                {
                    uptime = String.Format("{0:dd\\:hh\\:mm\\:ss}", (DateTime.Now - Convert.ToDateTime(startTime)));
                }
                else
                {
                    uptime = "N/A";
                }
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
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

        public ServiceController Service
        {
            get { return svc; }
        }

        public string Started
        {
            get { return startTime; }
        }

        public string Uptime
        {
            get { return uptime; }
        }
        
        public bool ChangesPending
        {
            get { return _serviceState != _originalServiceState; }
        }

        public bool Selected
        {
            get { return _selected; }
            set { SetField(ref _selected, value); }
        }

        public void StartService(ServiceController svc)
        {
            ApplicationThreadHelper.Invoke(() =>
            {
                StartServiceCommand cmd = new StartServiceCommand(svc);

                if (cmd.CanExecute(null))
                {
                    cmd.Execute(null);
                }
            });
        }

        public void StopService(ServiceController svc)
        {
            ApplicationThreadHelper.Invoke(() =>
            {
                StopServiceCommand cmd = new StopServiceCommand(svc);

                if (cmd.CanExecute(null))
                {
                    cmd.Execute(null);
                }
            });
        }

        public void RestartService(ServiceController svc)
        {
            ApplicationThreadHelper.Invoke(() =>
            {
                RestartServiceCommand cmd = new RestartServiceCommand(svc);

                if (cmd.CanExecute(null))
                {
                    cmd.Execute(null);
                }
            });
        }
    }
}
