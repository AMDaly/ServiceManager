﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using PeekServiceMonitor.ViewModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PeekServiceMonitor.View
{
    /// <summary>
    /// Interaction logic for LogView.xaml
    /// </summary>
    public partial class LogView : Window
    {
        public LogView()
        {
            InitializeComponent();
            
            DataContext = LogViewModel._logEntries;
        }
    }
}
