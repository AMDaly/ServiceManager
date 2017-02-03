using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Threading.Tasks;
using PeekServiceMonitor.PropertyChanged;
using PeekServiceMonitor.Commands;
using PeekServiceMonitor.View;
using log4net;
using System.Windows;

namespace PeekServiceMonitor.ViewModel
{
    public class MainWindowViewModel : NotifyPropertyChangedBase
    {
        private IServiceRunningViewModel _selectedService;
        private bool _servicesStopped;
        private ProcessExtensions processExtensions = new ProcessExtensions();
        private readonly ILog logger;
        private LogView logView = new LogView();
        private EditServicesView editServicesView = new EditServicesView();
        private EditServicesViewModel editServicesViewModel = new EditServicesViewModel();
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
            OpenEditServicesCommand = new RelayCommand(o => OpenEditServices(), p => true);
            OpenServiceLogCommand = new RelayCommand(o => OpenServiceLog(), p => _services.Services.Count > 0);
            HideWindowCommand = new RelayCommand(o => HideWindow(), p => App.mainView.Visibility == Visibility.Visible);
        }

        public ICommand StartAllServicesCommand { get; private set; }
        public ICommand StopAllServicesCommand { get; private set; }
        public ICommand RestartAllServicesCommand { get; private set; }
        public ICommand OpenEditServicesCommand { get; private set; }
        public ICommand OpenServiceLogCommand { get; private set; }
        public ICommand HideWindowCommand { get; private set; }

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

        public void HideEditServices()
        {
            if (editServicesView.IsVisible)
            {
                editServicesView.Hide();
            }
        }

        public void OpenServiceLog()
        {
            App.builder.CaptureEvents();

            if (!logView.IsVisible)
            {
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

        public bool EventsExist
        {
            get
            {
               return (App.logViewModel.EventCount > 0) ? true : false;
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

        public bool ServicesStopped
        {
            get { return _servicesStopped; }
            set { SetField(ref _servicesStopped, value); }
        }

        public void ShowWindow()
        {
            if (!App.mainView.IsVisible)
            {
                App.mainView.Show();
            }
        }

        public void HideWindow()
        {
            if (App.mainView.IsVisible)
            {
                App.mainView.Hide();
            }
        }
    }
}
