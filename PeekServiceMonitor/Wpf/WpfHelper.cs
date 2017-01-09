using System;
using System.Windows;
using System.Windows.Input;

namespace PeekServiceMonitor.Wpf
{
    public static class WpfHelper
    {
        public static void TriggerInvalidateRequerySuggestedOnUiThread()
        {
            var application = Application.Current;
            if (application == null)
            {
                CommandManager.InvalidateRequerySuggested();
            }
            else
            {
                application.Dispatcher.BeginInvoke(new Action(CommandManager.InvalidateRequerySuggested));
            }
        }
    }
}