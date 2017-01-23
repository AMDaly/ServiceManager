using log4net;
using PeekServiceMonitor.PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeekServiceMonitor.ViewModel
{
    class AddedServiceViewModel : NotifyPropertyChangedBase, IAddedServiceViewModel
    {

        private string desiredSvcName;
        private ILog logger;

        public AddedServiceViewModel(string desiredSvcName)
        {
            this.desiredSvcName = desiredSvcName;
        }

        public string Name
        {
            get
            {
                return this.desiredSvcName;
            }
        }
    }
}
