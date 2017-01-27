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
        private IServiceRunningViewModel _selectedService;
        private ProcessExtensions processExtensions = new ProcessExtensions();
        private readonly ILog logger;
        private LogView logView = new LogView();
        private EditServicesView editServicesView = new EditServicesView();
        public EditServicesViewModel editServicesViewModel = new EditServicesViewModel();
        private LogEntryBuilder builder = new LogEntryBuilder();
        private readonly PeekServiceCollection _services;

        public MainWindowViewModel(ICommand onInitializeCommand, PeekServiceCollection peekServiceCollection)
        {
            logger = LogManager.GetLogger(typeof(MainWindowViewModel));

            Task.Run(() =>
            {
                onInitializeCommand.Execute(this);
            });

            _services = peekServiceCollection;
            StartAllServicesCommand = new RelayCommand(o => _services.StartAllServices(), p => _services.Services.Count > 0);
            StopAllServicesCommand = new RelayCommand(o => _services.StopAllServices(), p => _services.Services.Count > 0);
            RestartAllServicesCommand = new RelayCommand(o => _services.RestartAllServices(), p => _services.Services.Count > 0);
            OpenEditServicesCommand = new RelayCommand(o => OpenEditServices(), p => _services.Services.Count > 0);
            OpenServiceLogCommand = new RelayCommand(o => OpenServiceLog(), p => _services.Services.Count > 0);
        }

        public ICommand StartAllServicesCommand { get; private set; }
        public ICommand StopAllServicesCommand { get; private set; }
        public ICommand RestartAllServicesCommand { get; private set; }
        public ICommand OpenEditServicesCommand { get; private set; }
        public ICommand OpenServiceLogCommand { get; private set; }

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

        public ObservableCollection<IServiceRunningViewModel> Services
        {
            get { return _services.Services; }
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
