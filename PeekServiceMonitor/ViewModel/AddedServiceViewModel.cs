using log4net;
using PeekServiceMonitor.Commands;
using PeekServiceMonitor.PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PeekServiceMonitor.ViewModel
{
    public class AddedServiceViewModel : NotifyPropertyChangedBase, IAddedServiceViewModel
    {
        private string desiredSvcName;
        private ILog logger;

        public AddedServiceViewModel(string desiredSvcName)
        {
            logger = LogManager.GetLogger(typeof(AddedServiceViewModel));
            this.desiredSvcName = desiredSvcName;
        }

        public string Name
        {
            get
            {
                return desiredSvcName;
            }
        }
    }
}
