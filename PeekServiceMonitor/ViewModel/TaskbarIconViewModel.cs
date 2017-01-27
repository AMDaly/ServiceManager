﻿using PeekServiceMonitor.PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using System.Windows.Input;
using PeekServiceMonitor.Commands;

namespace PeekServiceMonitor.ViewModel
{
    public class TaskbarIconViewModel : NotifyPropertyChangedBase
    {
        private readonly ILog logger;

        public TaskbarIconViewModel()
        {
            logger = LogManager.GetLogger(typeof(TaskbarIconViewModel));

            ShowMainWindowCommand = new RelayCommand(o => ShowMainWindow(), p => !App.mainView.IsVisible);
            HideMainWindowCommand = new RelayCommand(o => HideMainWindow(), p => App.mainView.IsVisible);
        }

        public ICommand ShowMainWindowCommand { get; private set; }
        public ICommand HideMainWindowCommand { get; private set; }

        public void ShowMainWindow()
        {
            App.mainView.Show();
        }

        public void HideMainWindow()
        {
            App.mainView.Hide();
        }
    }
}
