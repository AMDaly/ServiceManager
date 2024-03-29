﻿using System;
using System.ServiceProcess;
using PeekServiceMonitor.PropertyChanged;
using log4net;
using PeekServiceMonitor.Commands;
using System.Windows.Input;
using System.Threading;

namespace PeekServiceMonitor.ViewModel
{
    public class ServiceRunningViewModel : NotifyPropertyChangedBase, IServiceRunningViewModel
    {
        private readonly ILog logger;
        private readonly string _svcName;
        private readonly ServiceControllerStatus _originalServiceState;
        private ServiceControllerStatus _serviceState;
        private int id;
        private string startTime;
        private string uptime;
        private bool _selected;

        public readonly ProcessExtensions processExtensions = new ProcessExtensions();

        public ServiceRunningViewModel(String svcName)
        {
            logger = LogManager.GetLogger(typeof(ServiceRunningViewModel));

            Service = new ServiceController(svcName);
            _serviceState = Service.Status;

            StartServiceCommand = new RelayCommand(o => StartService(Service), p => Status == ServiceControllerStatus.Stopped);
            StopServiceCommand = new RelayCommand(o => StopService(Service), p => Status == ServiceControllerStatus.Running);
            RestartServiceCommand = new RelayCommand(o => RestartService(Service), p => Status == ServiceControllerStatus.Running);

            id = processExtensions.GetProcessId(Service);
            startTime = processExtensions.GetStartTime(id);

            if (Service.Status == ServiceControllerStatus.Running)
            {
                uptime = String.Format("{0:dd\\:hh\\:mm\\:ss}", (DateTime.Now - Convert.ToDateTime(startTime)));
            }
            else
            {
                uptime = "N/A";
            }
        }
        
        public string ServiceName
        {
            get { return Service.ServiceName; }
        }

        public string DisplayName
        {
            get { return Service.DisplayName; }
        }

        public ServiceControllerStatus Status
        {
            get { return _serviceState; }
            set { SetField(ref _serviceState, value); }
        }

        public ServiceController Service { get; }

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
                try
                {
                    logger.Info($"Starting service {svc.DisplayName}. Current status: {Status}");
                    svc.Start();
                    svc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
                    Uptime = "";
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                    break;
                }
                
                Thread.CurrentThread.Join(100);
            }

            var id = processExtensions.GetProcessId(Service);
            Started = processExtensions.GetStartTime(id);
            svc.Refresh();
            Status = svc.Status;
            logger.Info($"{svc.DisplayName} current status: {svc.Status}");
        }

        public void StopService(ServiceController svc)
        {
            svc.Refresh();
            logger.Info($"Stopping service {svc.ServiceName}. Current status: {svc.Status}.");

            var waitUntil = DateTime.UtcNow.AddSeconds(15);
            while (svc.Status != ServiceControllerStatus.Stopped && DateTime.UtcNow < waitUntil)
            {
                try
                {
                    logger.Info($"Stopping service {svc.DisplayName}. Current status: {Status}");
                    svc.Stop();
                    svc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
                    Uptime = "";
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                    break;
                }
                
                Thread.CurrentThread.Join(100);
            }

            svc.Refresh();
            Status = svc.Status;
            logger.Info($"{svc.DisplayName} current status: {svc.Status}");
        }

        public void RestartService(ServiceController svc)
        {
            StopService(svc);
            StartService(svc);
        }

        public void UpdateState()
        {
            if (Service == null) return;

            Service.Refresh();

            Status = Service.Status;

            if (Service.Status == ServiceControllerStatus.Running)
            {
                if (Started != "N/A")
                {
                    Uptime = String.Format("{0:dd\\:hh\\:mm\\:ss}", (DateTime.Now - Convert.ToDateTime(Started)));
                }
            }
            else
            {
                Uptime = "N/A";
            }
        }
    }
}
