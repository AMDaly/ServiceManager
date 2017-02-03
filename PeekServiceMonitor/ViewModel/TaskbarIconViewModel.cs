using PeekServiceMonitor.PropertyChanged;
using System.Windows;
using log4net;
using System.Windows.Input;
using PeekServiceMonitor.Commands;
using PeekServiceMonitor.Controls;
using System.Collections.ObjectModel;

namespace PeekServiceMonitor.ViewModel
{
    public class TaskbarIconViewModel : NotifyPropertyChangedBase
    {
        private readonly ILog logger;
        private PeekServiceCollection peekServiceCollection;
        private TaskbarPopup taskbarPopup;

        public TaskbarIconViewModel(PeekServiceCollection peekServiceCollection)
        {
            logger = LogManager.GetLogger(typeof(TaskbarIconViewModel));

            this.peekServiceCollection = peekServiceCollection;

            taskbarPopup = new TaskbarPopup();
            
            ShowMainWindowCommand = new RelayCommand(o => ShowMainWindow(), p => !App.mainView.IsVisible);
            HideMainWindowCommand = new RelayCommand(o => HideMainWindow(), p => App.mainView.IsVisible);
            ExitApplicationCommand = new RelayCommand(o => ExitApplication(), p => Application.Current.MainWindow.IsInitialized);
        }

        public ICommand ShowPopupCommand { get; private set; }
        public ICommand ShowMainWindowCommand { get; private set; }
        public ICommand HideMainWindowCommand { get; private set; }
        public ICommand ExitApplicationCommand { get; private set; }

        public ObservableCollection<IServiceRunningViewModel> Services
        {
            get { return peekServiceCollection.Services; }
        }

        public void ShowMainWindow()
        {
            App.mainView.Show();
        }

        public void HideMainWindow()
        {
            App.mainView.Hide();
        }
        
        public void ExitApplication()
        {
            Application.Current.MainWindow.Close();
        }
    }
}
