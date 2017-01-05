using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Threading.Tasks;

namespace PeekServiceMonitor.ViewModel
{
    class MainWindowViewModel
    {
        private readonly ObservableCollection<IServiceRunningViewModel> _services = new ObservableCollection<IServiceRunningViewModel>();


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
            Task AddService = new Task(() =>
            {
                var service = Services.FirstOrDefault(
                    p => string.Equals(serviceViewModel.Name, p.Name, StringComparison.InvariantCulture));

                Services.Add(service);
            });

            AddService.Start();
            AddService.Wait();
        }
    }
}
