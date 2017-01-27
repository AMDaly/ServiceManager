using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceProcess;
using System.Timers;
using PeekServiceMonitor.Wpf;

namespace PeekServiceMonitor.ViewModel
{
    public class PeekServiceCollection
    {
        private readonly Timer _timer = new Timer();
        private readonly ObservableCollection<IServiceRunningViewModel> _services = new ObservableCollection<IServiceRunningViewModel>();

        public PeekServiceCollection()
        {
            _timer.Interval = 200;
            _timer.Elapsed += Timer_Elapsed;
            _timer.Start();
        }

        public ObservableCollection<IServiceRunningViewModel> Services
        {
            get { return _services; }
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            foreach (var svc in _services.ToList())
            {
                svc.UpdateState();
            }
        }

        public void Add(ServiceRunningViewModel serviceViewModel)
        {
            ApplicationThreadHelper.Invoke(() =>
            {
                _services.Add(serviceViewModel);
            });
        }

        public void StartAllServices()
        {
            foreach (var svc in _services)
            {
                var svcViewModel = new ServiceRunningViewModel(svc.ServiceName);

                if (svcViewModel.Status == ServiceControllerStatus.Stopped)
                {
                    svcViewModel.StartService(svc.Service);
                    svcViewModel.Service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(15));
                }
            }
        }

        public void StopAllServices()
        {
            foreach (var svc in _services)
            {
                var svcViewModel = new ServiceRunningViewModel(svc.ServiceName);

                if (svcViewModel.Status == ServiceControllerStatus.Running)
                {
                    svcViewModel.StopService(svc.Service);
                    svcViewModel.Service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(15));
                }
            }
        }

        public void RestartAllServices()
        {
            foreach (var svc in _services)
            {
                var svcViewModel = new ServiceRunningViewModel(svc.ServiceName);

                svcViewModel.RestartService(svc.Service);
                svcViewModel.Service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(15));
            }
        }
    }
}