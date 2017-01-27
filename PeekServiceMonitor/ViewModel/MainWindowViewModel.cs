using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Threading.Tasks;
using PeekServiceMonitor.PropertyChanged;
using PeekServiceMonitor.Wpf;
using PeekServiceMonitor.Commands;
using PeekServiceMonitor.View;
using System.ServiceProcess;
using System.Timers;
using System.Linq;
using System.IO;
using log4net;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Diagnostics.Eventing.Reader;
using System.Windows;
using PeekServiceMonitor.Util;

namespace PeekServiceMonitor.ViewModel
{
    public class MainWindowViewModel : NotifyPropertyChangedBase
    {
        public ObservableCollection<IServiceRunningViewModel> _services = new ObservableCollection<IServiceRunningViewModel>();
        private IServiceRunningViewModel _selectedService;
        private ProcessExtensions processExtensions = new ProcessExtensions();
        public Timer timer = new Timer();
        private readonly ILog logger;
        private LogView logView = new LogView();
        private EditServicesView editServicesView = new EditServicesView();
        public EditServicesViewModel editServicesViewModel = new EditServicesViewModel();
        private LogEntryBuilder builder = new LogEntryBuilder();

        public MainWindowViewModel(ICommand onInitializeCommand)
        {
            logger = LogManager.GetLogger(typeof(MainWindowViewModel));

            Task.Run(() =>
            {
                onInitializeCommand.Execute(this);
            });

            StartAllServicesCommand = new RelayCommand(o => StartAllServices(), p => _services.Count > 0);
            StopAllServicesCommand = new RelayCommand(o => StopAllServices(), p => _services.Count > 0);
            RestartAllServicesCommand = new RelayCommand(o => RestartAllServices(), p => _services.Count > 0);
            OpenEditServicesCommand = new RelayCommand(o => OpenEditServices(), p => _services.Count > 0);
            OpenServiceLogCommand = new RelayCommand(o => OpenServiceLog(), p => _services.Count > 0);

            timer.Interval = 200;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        public ICommand StartAllServicesCommand { get; private set; }
        public ICommand StopAllServicesCommand { get; private set; }
        public ICommand RestartAllServicesCommand { get; private set; }
        public ICommand OpenEditServicesCommand { get; private set; }
        public ICommand OpenServiceLogCommand { get; private set; }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            foreach (var svc in _services.ToList())
            {
                svc.UpdateState();
            }
        }

        public ObservableCollection<IServiceRunningViewModel> Services
        {
            get { return _services; }
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
        
        public void OpenEditServices()
        {
            if (!editServicesView.IsVisible)
            {
                if (editServicesView.IsLoaded)
                {
                    editServicesView.Show();
                }
                else
                {
                    editServicesView = new EditServicesView()
                    {
                        DataContext = editServicesViewModel
                    };

                    editServicesView.Show();
                }
            }
            else
            {
                editServicesView.Activate();
            }
        }

        public void OpenServiceLog()
        {
            if (!logView.IsVisible)
            {
                builder.CaptureEvents();

                if (logView.IsLoaded)
                {
                    logView.Show();
                }
                else
                {
                    logView = new LogView();
                    logView.Show();
                }
            }
            else
            {
                logView.Activate();
            }
        }

        public IServiceRunningViewModel SelectedService
        {
            get { return _selectedService; }
            set { SetField(ref _selectedService, value); }
        }

        public void ShowWindow()
        {
            if (App.mainView.IsVisible)
            {
                App.mainView.Show();
            }
        }

        public void HideWindow()
        {
            if (!App.mainView.IsVisible)
            {
                App.mainView.Hide();
            }
        }
    }
}
