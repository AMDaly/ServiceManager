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
        String Name
        {
            get;
        }

        ServiceControllerStatus Status
        {
            get; set;
        }
    }
}
