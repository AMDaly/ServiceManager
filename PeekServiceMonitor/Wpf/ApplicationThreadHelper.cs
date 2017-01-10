using System;
using System.Windows;

namespace PeekServiceMonitor.Wpf
{
    public static class ApplicationThreadHelper
    {
        public static void Invoke(Action action)
        {
            try
            {
                var application = Application.Current;
                var dispatcher = application != null ? application.Dispatcher : null;
                if (dispatcher != null)
                {
                    dispatcher.Invoke(action);
                }
                else
                {
                    action();
                }
            }
            catch (Exception e)
            {
                //typeof(ApplicationThreadHelper).Log().Error("Exception in invoked action.", e);
                throw;
            }
        }
    }
}
