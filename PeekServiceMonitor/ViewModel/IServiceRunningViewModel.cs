﻿using System.ServiceProcess;

namespace PeekServiceMonitor.ViewModel
{
    public interface IServiceRunningViewModel
    {
        string ServiceName
        {
            get;
        }

        string DisplayName
        {
            get;
        }

        ServiceControllerStatus Status
        {
            get; set;
        }

        string Started
        {
            get; set;
        }

        string Uptime
        {
            get; set;
        }

        ServiceController Service
        {
            get;
        }

        bool Selected
        {
            get; set;
        }

        void StartService(ServiceController svc);

        void StopService(ServiceController svc);

        void RestartService(ServiceController svc);

        void UpdateState();
    }
}
