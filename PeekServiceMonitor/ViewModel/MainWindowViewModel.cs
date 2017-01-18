using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Threading.Tasks;
using PeekServiceMonitor.PropertyChanged;
using PeekServiceMonitor.Wpf;
using PeekServiceMonitor.Commands;
using System.ServiceProcess;

namespace PeekServiceMonitor.ViewModel
{
    class MainWindowViewModel : NotifyPropertyChangedBase
    {
        private readonly ObservableCollection<IServiceRunningViewModel> _services = new ObservableCollection<IServiceRunningViewModel>();
        private IServiceRunningViewModel _selectedService;

        public MainWindowViewModel(ICommand onInitializeCommand)
        {
            Task.Run(() =>
            {
                onInitializeCommand.Execute(this);
            });
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

        public IServiceRunningViewModel SelectedService
        {
            get { return _selectedService; }
            set { SetField(ref _selectedService, value); }
        }
    }
}
