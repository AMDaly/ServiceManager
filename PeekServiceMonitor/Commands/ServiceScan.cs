using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PeekServiceMonitor.ViewModel;
using log4net;
using System.Windows.Input;

namespace PeekServiceMonitor.Commands
{
    class ServiceScan : WpfCommandBase<MainWindowViewModel>
    {
        List<String> initialSvcList = new List<String>();
        private readonly ILog logger;

        public ServiceScan()
        {
            logger = LogManager.GetLogger(typeof(ServiceScan));
        }

        protected override void ExecuteInternal(MainWindowViewModel parameter)
        {

            logger.Info("Inside ExecuteServiceScan");

            initialSvcList.Add("PeekStateMachineService_V1.7.1");
            initialSvcList.Add("PeekSchedulerService_V1.7.1");

            Task.Run(() =>
            {
                foreach (var svc in initialSvcList)
                {
                    try
                    {
                        logger.Info("Adding Service..");
                        parameter.Add(new ServiceRunningViewModel(svc));
                        logger.Info("Service added.");
                    }
                    catch (Exception ex)
                    {
                        logger.Warn(string.Format("Failed to add Service: {0}.", svc), ex);
                        logger.Warn(ex.InnerException);
                    }
                }
            }).Wait();
        }
    }
}
