using log4net;
using PeekServiceMonitor.PropertyChanged;

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
