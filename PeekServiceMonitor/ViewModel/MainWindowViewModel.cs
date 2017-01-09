﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Threading.Tasks;
using PeekServiceMonitor.PropertyChanged;

namespace PeekServiceMonitor.ViewModel
{
    class MainWindowViewModel : NotifyPropertyChangedBase
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
            Task.Run(() =>
            {
                _services.Add(serviceViewModel);
            });
        }
    }
}
