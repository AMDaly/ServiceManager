using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PeekServiceMonitor.ViewModel;
using log4net;
using System.Windows.Input;
using System.ServiceProcess;
using System.Text.RegularExpressions;

namespace PeekServiceMonitor.Commands
{
    class ServiceScan : WpfCommandBase<MainWindowViewModel>
    {
        private List<ServiceController> peekSvcList = new List<ServiceController>();
        private readonly ILog logger;

        public ServiceScan()
        {
            logger = LogManager.GetLogger(typeof(ServiceScan));
        }

        protected override void ExecuteInternal(MainWindowViewModel parameter)
        {
            List<ServiceController> allSvcList = ServiceController.GetServices().ToList();

            peekSvcList = allSvcList
                .Where(p => Regex.IsMatch(p.ServiceName, "peek", RegexOptions.IgnoreCase)
                            || Regex.IsMatch(p.ServiceName, "spinnaker", RegexOptions.IgnoreCase)
                            || Regex.IsMatch(p.ServiceName, "semex", RegexOptions.IgnoreCase)).ToList();
            
            Task.Run(() =>
            {
                foreach (var svc in peekSvcList)
                {
                    var svcName = svc.ServiceName;
                    try
                    {
                        logger.Info($"Adding Service {svcName}");
                        parameter.Services.Add(new ServiceRunningViewModel(svc.ServiceName));
                        logger.Info($"Service {svcName} added.");
                    }
                    catch (Exception ex)
                    {
                        logger.Warn(string.Format($"Failed to add Service: {svcName}."), ex);
                        logger.Warn(ex.InnerException);
                    }
                }
            });
        }
    }
}