using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceProcess;
using System.Timers;
using PeekServiceMonitor.Wpf;
using System.Windows;
using System.Threading;
using System.Windows.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Data;
using PeekServiceMonitor.Properties;

namespace PeekServiceMonitor.ViewModel
{
    public class PeekServiceCollection
    {
        private bool urgentIcon = false;
        private readonly System.Timers.Timer _timer = new System.Timers.Timer();
        private readonly ObservableCollection<IServiceRunningViewModel> _services = new ObservableCollection<IServiceRunningViewModel>();
        private object _lock = new object();

        public PeekServiceCollection()
        {
            BindingOperations.EnableCollectionSynchronization(_services, _lock);
            
            _timer.Interval = 200;
            _timer.Elapsed += Timer_Elapsed;
        }

        public ObservableCollection<IServiceRunningViewModel> Services
        {
            get { return _services; }
        }

        public void StartTimer()
        {
            _timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                foreach (var svc in _services.ToList())
                {
                    svc.UpdateState();
                }
            }
            catch (Exception)
            {
                
            }
            
            if (App.viewModel != null)
            {
                App.viewModel.ServicesStopped = IsAnyServiceStopped();
            }

            UpdateIcon();
        }
        
        private void Update(object svc)
        {
            ((IServiceRunningViewModel)svc).UpdateState();
        }

        public void Add(ServiceRunningViewModel serviceViewModel)
        {
            ApplicationThreadHelper.Invoke(() =>
            {
                try
                {
                    _services.Add(serviceViewModel);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }    
            });
        }

        public void StartAllServices()
        {
            Task.Factory.StartNew(() =>
            {
                Parallel.ForEach<IServiceRunningViewModel>(
                    _services,
                    (o) =>
                    {
                        var svc = new ServiceRunningViewModel(o.ServiceName);

                        try
                        {
                            svc.StartService(svc.Service);
                            svc.Service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(15));
                        }
                        catch (System.ServiceProcess.TimeoutException)
                        {
                            //Activate balloon
                            Debug.WriteLine($"Failed to start service: {svc.DisplayName}");
                        }
                    });
            });
        }

        public void StopAllServices()
        {
            Task.Factory.StartNew(() =>
            {
                Parallel.ForEach<IServiceRunningViewModel>(
                    _services,
                    (o) =>
                    {
                        var svc = new ServiceRunningViewModel(o.ServiceName);

                        try
                        {
                            Debug.WriteLine($"Stopping service: {svc.DisplayName}");
                            svc.StopService(svc.Service);
                            svc.Service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(15));
                        }
                        catch (System.ServiceProcess.TimeoutException)
                        {
                            //Activate balloon
                            Debug.WriteLine($"Failed to stop service: {svc.DisplayName}");
                        }
                    });
            });
        }

        public void RestartAllServices()
        {
            Task.Factory.StartNew(() =>
            {
                Parallel.ForEach<IServiceRunningViewModel>(
                    _services,
                    (o) =>
                    {
                        var svc = new ServiceRunningViewModel(o.ServiceName);

                        try
                        {
                            svc.RestartService(svc.Service);
                            svc.Service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(15));
                        }
                        catch (System.ServiceProcess.TimeoutException)
                        {
                            //Activate balloon
                            Debug.WriteLine($"Failed to restart service: {svc.DisplayName}");
                        }
                    });
            });
        }

        public bool IsAnyServiceStopped()
        {
            foreach (var svc in _services)
            {
                if (svc.Status == ServiceControllerStatus.Stopped)
                {
                    return true;
                }
            }

            return false;
        }

        public void UpdateIcon()
        {
            if (App.tb == null)
            {
                return;
            }
            
            if (App.viewModel.ServicesStopped && !urgentIcon)
            {
                App.tb.Icon = Resources.IconAppUrgent;
                urgentIcon = true;
            }
            else if (!App.viewModel.ServicesStopped && urgentIcon)
            {
                App.tb.Icon = Resources.IconAppCool;
                urgentIcon = false;
            }
        }
    }
}