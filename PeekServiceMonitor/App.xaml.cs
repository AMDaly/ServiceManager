using System.Windows;
using PeekServiceMonitor.Commands;
using log4net;
using log4net.Config;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PeekServiceMonitor.ViewModel;
using System.Threading;
using PeekServiceMonitor.Properties;

namespace PeekServiceMonitor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly ILog logger;
        private MainWindowViewModel viewModel;
        List<String> initialSvcList = new List<String>();

        public App()
        {
            logger = LogManager.GetLogger(typeof(App));

            var assembly = Assembly.GetExecutingAssembly();
            logger.Info(assembly.ToString());
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            logger.Info("Application_Startup");

            viewModel = new MainWindowViewModel(new ServiceScan());
            
            if (Settings.Default.AddedServices != null && Settings.Default.AddedServices.Count > 0)
            {
                foreach (var svc in Settings.Default.AddedServices)
                {
                    viewModel.Add(new ServiceRunningViewModel(svc));
                }
            }

            MainWindow = new MainWindow { DataContext = viewModel };
            MainWindow.Show();
        }

        private void OnMainWindowClosing()
        {
            Task.Run(() =>
            {
                viewModel.timer.Stop();
            }).Wait();
        }
    }
}
