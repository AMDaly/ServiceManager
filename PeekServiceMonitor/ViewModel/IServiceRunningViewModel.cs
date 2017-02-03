using System;
using System.ServiceProcess;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        void UpdateState();
    }
}
