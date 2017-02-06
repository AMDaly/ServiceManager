using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceProcess;
using System.Timers;
using PeekServiceMonitor.Wpf;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Data;
using PeekServiceMonitor.Properties;
using log4net;

namespace PeekServiceMonitor.ViewModel
{
    public class PeekServiceCollection
    {
        private bool urgentIcon = false;
        private readonly Timer _timer = new Timer();
        private readonly ObservableCollection<IServiceRunningViewModel> _services = new ObservableCollection<IServiceRunningViewModel>();
        private object _lock = new object();
        private readonly ILog logger;

        public PeekServiceCollection()
        {
            logger = LogManager.GetLogger(typeof(PeekServiceCollection));

            BindingOperations.EnableCollectionSynchronization(_services, _lock);
            
            _timer.Interval = 500;
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
            _timer.Stop();

            Task.Factory.StartNew(() =>
            {
                Parallel.ForEach<IServiceRunningViewModel>(
                    _services,
                    (svc) =>
                    {
                        svc.StartService(svc.Service);
                    });

                Timer_Elapsed(null, null);
                _timer.Start();
            });

           
        }

        public void StopAllServices()
        {
            _timer.Stop();

            Task.Factory.StartNew(() =>
            {
                Parallel.ForEach<IServiceRunningViewModel>(
                    _services,
                    (svc) =>
                    {
                        svc.StopService(svc.Service);
                    });

                Timer_Elapsed(null, null);
                _timer.Start();
            });

            
        }

        public void RestartAllServices()
        {
            _timer.Stop();

            Task.Factory.StartNew(() =>
            {
                Parallel.ForEach<IServiceRunningViewModel>(
                    _services,
                    (svc) =>
                    {
                        svc.RestartService(svc.Service);
                        
                    });

                Timer_Elapsed(null, null);
                _timer.Start();
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