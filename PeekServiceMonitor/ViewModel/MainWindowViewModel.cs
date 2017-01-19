﻿using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Threading.Tasks;
using PeekServiceMonitor.PropertyChanged;
using PeekServiceMonitor.Wpf;
using PeekServiceMonitor.Commands;
using System.ServiceProcess;
using System.Timers;
using System.Linq;
using System.IO;
using log4net;

namespace PeekServiceMonitor.ViewModel
{
    class MainWindowViewModel : NotifyPropertyChangedBase
    {
        private readonly ObservableCollection<IServiceRunningViewModel> _services = new ObservableCollection<IServiceRunningViewModel>();
        private IServiceRunningViewModel _selectedService;
        private ProcessExtensions processExtensions = new ProcessExtensions();
        public System.Timers.Timer timer = new System.Timers.Timer();
        private readonly ILog logger;

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
            ToggleSettingsVisibilityCommand = new RelayCommand(o => ToggleSettingsVisibility(), p => _services.Count > 0);
            OpenServiceLogCommand = new RelayCommand(o => OpenServiceLog(), p => _services.Count > 0);

            timer.Interval = 100;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        public ICommand StartAllServicesCommand { get; private set; }
        public ICommand StopAllServicesCommand { get; private set; }
        public ICommand RestartAllServicesCommand { get; private set; }
        public ICommand ToggleSettingsVisibilityCommand { get; private set; }
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
                var svcViewModel = new ServiceRunningViewModel(svc.Name);

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
                var svcViewModel = new ServiceRunningViewModel(svc.Name);

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
                var svcViewModel = new ServiceRunningViewModel(svc.Name);

                svcViewModel.RestartService(svc.Service);
                svcViewModel.Service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(15));
            }
        }

        public void ToggleSettingsVisibility()
        {

        }

        public void OpenServiceLog()
        {
            string peekDataPath = Environment.ExpandEnvironmentVariables("%PEEKDATAFOLDER%");
            string logPath = $"{peekDataPath}\\Log\\PeekServiceManager.log";

            try
            {
                File.Open(logPath, FileMode.Open);
            }
            catch (FileNotFoundException ex)
            {
                logger.Warn("Cannot open log file.", ex);
            }
        }

        public IServiceRunningViewModel SelectedService
        {
            get { return _selectedService; }
            set { SetField(ref _selectedService, value); }
        }
    }
}
