using System;
using System.Diagnostics;
using System.ServiceProcess;
using PeekServiceMonitor.PropertyChanged;
using log4net;
using PeekServiceMonitor.Commands;
using PeekServiceMonitor.Wpf;
using System.Windows.Input;
using System.Threading;

namespace PeekServiceMonitor.ViewModel
{
    public class ServiceRunningViewModel : NotifyPropertyChangedBase, IServiceRunningViewModel
    {
        private readonly ILog logger;
        private readonly ServiceController svc;
        private readonly ServiceControllerStatus _originalServiceState;
        private ServiceControllerStatus _serviceState;
        private Process process;
        private int id;
        private string startTime;
        private string uptime;
        private bool _selected;

        private ProcessExtensions processExtensions = new ProcessExtensions();

        public ServiceRunningViewModel(String svcName)
        {
            logger = LogManager.GetLogger(typeof(ServiceRunningViewModel));
            ProcessExtensions processExtensions = new ProcessExtensions();
            
            try
            {
                svc = new ServiceController(svcName);

                StartServiceCommand = new RelayCommand(o => StartService(svc), p => Status == ServiceControllerStatus.Stopped);
                StopServiceCommand = new RelayCommand(o => StopService(svc), p => Status == ServiceControllerStatus.Running);
                RestartServiceCommand = new RelayCommand(o => RestartService(svc), p => Status == ServiceControllerStatus.Running);

                _serviceState = svc.Status;
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
            get { return _serviceState; }
            set { SetField(ref _serviceState, value); }
        }

        public ServiceController Service
        {
            get { return svc; }
        }

        public string Started
        {
            get { return startTime; }
            set { SetField(ref startTime, value); }
        }

        public string Uptime
        {
            get { return uptime; }
            set { SetField(ref uptime, value); }
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

        public ICommand StartServiceCommand { get; private set; }
        public ICommand StopServiceCommand { get; private set; }
        public ICommand RestartServiceCommand { get; private set; }

        public void StartService(ServiceController svc)
        {
            logger.Info($"Starting service {svc.ServiceName}. Current status is {svc.Status}.");
            
            var waitUntil = DateTime.UtcNow.AddSeconds(15);
            while (svc.Status != ServiceControllerStatus.Running && svc.Status != ServiceControllerStatus.StartPending && DateTime.UtcNow < waitUntil)
            {
                svc.Start();
                svc.Refresh();
                Status = svc.Status;
                logger.Info($"Stopping service {svc.DisplayName}. Current status: {Status}");
                Thread.CurrentThread.Join(100);
            }

            svc.Refresh();
            logger.Info($"{svc.DisplayName} current status: {svc.Status}");
        }

        public void StopService(ServiceController svc)
        {
            svc.Refresh();
            logger.Info($"Stopping service {svc.ServiceName}. Current status: {svc.Status}.");

            var waitUntil = DateTime.UtcNow.AddSeconds(15);
            while (svc.Status != ServiceControllerStatus.Stopped && DateTime.UtcNow < waitUntil)
            {
                if (svc.Status == ServiceControllerStatus.Running)
                {
                    logger.Info($"Service {svc.DisplayName} is Running.");
                    svc.Stop();
                    svc.WaitForStatus(ServiceControllerStatus.StopPending);
                }

                svc.Refresh();
                Status = svc.Status;
                logger.Info($"Stopping service {svc.DisplayName}. Current status: {Status}");
                Thread.CurrentThread.Join(100);
            }

            logger.Info($"{svc.DisplayName} current status: {svc.Status}");
        }

        public void RestartService(ServiceController svc)
        {
            StopService(svc);
            StartService(svc);
        }

        public void UpdateState()
        {
            svc.Refresh();
            Status = svc.Status;

            if (svc.Status == ServiceControllerStatus.Running)
            {
                var id = processExtensions.GetProcessId(svc);
                Started = processExtensions.GetStartTime(id);
                Uptime = String.Format("{0:dd\\:hh\\:mm\\:ss}", (DateTime.Now - Convert.ToDateTime(Started)));
            }
            else
            {
                Uptime = "N/A";
            }
        }
    }
}
