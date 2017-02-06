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
using PeekServiceMonitor.Util;
using Hardcodet.Wpf.TaskbarNotification;
using System.ComponentModel;

namespace PeekServiceMonitor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly ILog logger;
        public static MainWindowViewModel viewModel;
        public static MainWindow mainView;
        public static TaskbarIcon tb;
        List<string> initialSvcList = new List<string>();
        public static LogViewModel logViewModel = new LogViewModel();
        public static LogEntryBuilder builder = new LogEntryBuilder();
        private readonly ServiceScan serviceScan = new ServiceScan();
        private readonly BackgroundWorker worker = new BackgroundWorker();
        private readonly PeekServiceCollection peekServiceCollection = new PeekServiceCollection();

        public App()
        {
            logger = LogManager.GetLogger(typeof(App));

            var assembly = Assembly.GetExecutingAssembly();
            logger.Info(assembly.ToString());
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            logger.Info("Application_Startup");
            
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            
            if (Settings.Default.AddedServices != null && Settings.Default.AddedServices.Count > 0)
            {
                foreach (var svc in Settings.Default.AddedServices)
                {
                    peekServiceCollection.Add(new ServiceRunningViewModel(svc));
                }
            }

            viewModel = new MainWindowViewModel(serviceScan, peekServiceCollection);

            builder.CaptureEvents();

            MainWindow = mainView = new MainWindow { DataContext = viewModel };
            MainWindow.Show();

            tb = (TaskbarIcon)FindResource("NotifyIcon");
            tb.Icon = PeekServiceMonitor.Properties.Resources.IconAppCool;
            tb.DataContext = new TaskbarIconViewModel(peekServiceCollection);

            peekServiceCollection.StartTimer();
        }

        private void OnMainWindowClosing()
        {
            Task.Run(() =>
            {
//                viewModel.PeekServiceCollection.timer.Stop();
            }).Wait();
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            MessageBox.Show(ex.Message, "Uncaught Thread Exception", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
