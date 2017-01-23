using log4net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PeekServiceMonitor.PropertyChanged;
using System.Text;
using System.Threading.Tasks;

namespace PeekServiceMonitor.ViewModel
{
    class EditServicesViewModel : PropertyChangedBase
    {
        public static ObservableCollection<IAddedServiceViewModel> _addedServices = new ObservableCollection<IAddedServiceViewModel>();
        private ILog logger;

        public EditServicesViewModel()
        {
            logger = LogManager.GetLogger(typeof(EditServicesViewModel));

            if (Properties.Settings.Default.AddedServices != null && Properties.Settings.Default.AddedServices.Count > 0)
            {
                foreach (var savedSvcName in Properties.Settings.Default.AddedServices)
                {
                    _addedServices.Add(new AddedServiceViewModel(savedSvcName));
                }
            }
        }

        public ObservableCollection<IAddedServiceViewModel> AddedServices
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
            var svcNames = new List<string>();

            svcNames = Properties.Settings.Default.AddedServices.Cast<string>().ToList();

            foreach (var str in svcNames)
            {
                if (desiredSvcName.IndexOf(str, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return;
                }
                else
                {
                    _addedServices.Add(new AddedServiceViewModel(desiredSvcName));
                }
            }
        }

        public void RemoveService(AddedServiceViewModel desiredSvc)
        {
            _addedServices.Remove(desiredSvc);
        }
    }
}
