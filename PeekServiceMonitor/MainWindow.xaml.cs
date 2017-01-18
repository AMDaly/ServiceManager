using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PeekServiceMonitor.Commands;
using PeekServiceMonitor.ViewModel;

namespace PeekServiceMonitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            var svc = ((MainWindowViewModel)DataContext).SelectedService;
            var model = new ServiceRunningViewModel(svc.Name);

            model.StartService(svc.Service);
        }

        private void StopSelectedButton_Click(object sender, RoutedEventArgs e)
        {
            var svc = ((MainWindowViewModel)DataContext).SelectedService;
            var model = new ServiceRunningViewModel(svc.Name);

            model.StopService(svc.Service);
        }

        private void RestartSelectedButton_Click(object sender, RoutedEventArgs e)
        {
            var svc = ((MainWindowViewModel)DataContext).SelectedService;
            var model = new ServiceRunningViewModel(svc.Name);

            model.RestartService(svc.Service);
        }
    }
}
