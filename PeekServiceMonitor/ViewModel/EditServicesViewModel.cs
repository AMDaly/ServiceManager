using log4net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PeekServiceMonitor.PropertyChanged;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using PeekServiceMonitor.Commands;
using System.Diagnostics;
using System.ServiceProcess;

namespace PeekServiceMonitor.ViewModel
{
    public class EditServicesViewModel : PropertyChangedBase
    {
        public static ObservableCollection<IAddedServiceViewModel> _addedServices = new ObservableCollection<IAddedServiceViewModel>();
        private ILog logger;

        public EditServicesViewModel()
        {
            logger = LogManager.GetLogger(typeof(EditServicesViewModel));

            Debug.WriteLine("Count: " + Properties.Settings.Default.AddedServices.Count);
            if (Properties.Settings.Default.AddedServices != null && Properties.Settings.Default.AddedServices.Count > 0)
            {
                foreach (var savedSvcName in Properties.Settings.Default.AddedServices)
                {
                    _addedServices.Add(new AddedServiceViewModel(savedSvcName));
                    logger.Info($"{savedSvcName} added to service list.");
                }
            }

            AddServiceCommand = new RelayCommand<string>(o => AddService(o), p => _addedServices.Count >= 0);
            RemoveServiceCommand = new RelayCommand<AddedServiceViewModel>(o => RemoveService(o), p => _addedServices.Count >= 0);
        }

        public ICommand AddServiceCommand { get; set; }
        public ICommand RemoveServiceCommand { get; set; }

        public static ObservableCollection<IAddedServiceViewModel> AddedServices
        {
            get
            {
                return _addedServices;
            }
            set
            {
                AddedServices = value;
            }
        }
        
        public void AddService(string desiredSvcName)
        {
            if (desiredSvcName.Equals(""))
            {
                return;
            }

            Debug.WriteLine("Inside AddService");
            var svcNames = new List<string>();

            svcNames = Properties.Settings.Default.AddedServices.Cast<string>().ToList();

            foreach (var str in svcNames)
            {
                if (desiredSvcName.IndexOf(str, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    logger.Info($"{desiredSvcName} already exists in the list of manually added services. Aborting add.");
                    return;
                }
            }

            logger.Info($"Adding {desiredSvcName} to the list of manually added services.");

            var matches = ServiceController.GetServices()
                .Where(p => p.ServiceName.Equals(desiredSvcName, StringComparison.OrdinalIgnoreCase) 
                    || p.DisplayName.Equals(desiredSvcName, StringComparison.OrdinalIgnoreCase));

            if (matches.Count() > 0)
            {
                Properties.Settings.Default.AddedServices.Add(desiredSvcName);
                _addedServices.Add(new AddedServiceViewModel(desiredSvcName));
                App.viewModel.Add(new ServiceRunningViewModel(desiredSvcName));
            }
        }
        
        public void RemoveService(AddedServiceViewModel desiredSvc)
        {
            logger.Info($"Removing {desiredSvc.Name} from the list of manually added services.");

            Properties.Settings.Default.AddedServices.Remove(desiredSvc.Name);
            _addedServices.Remove(desiredSvc);
            foreach (var x in App.viewModel._services)
            {
                Debug.WriteLine(x.ServiceName);
            }

            var serviceNameMatch = App.viewModel._services.Where(p => p.ServiceName.Equals(desiredSvc.Name, StringComparison.OrdinalIgnoreCase)).ToArray();
            var displayNameMatch = App.viewModel._services.Where(p => p.DisplayName.Equals(desiredSvc.Name, StringComparison.OrdinalIgnoreCase)).ToArray();

            if (serviceNameMatch.Count() == 1)
            {
                App.viewModel._services.Remove(serviceNameMatch[0]);
            }
            else if (displayNameMatch.Count() == 1)
            {
                App.viewModel._services.Remove(displayNameMatch[0]);
            }
        }
    }
}
