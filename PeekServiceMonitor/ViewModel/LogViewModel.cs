using PeekServiceMonitor.PropertyChanged;
using PeekServiceMonitor.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace PeekServiceMonitor.ViewModel
{
    public class LogViewModel : PropertyChangedBase
    {
        
        public static ObservableCollection<CollapsibleLogEntry> _logEntries = new ObservableCollection<CollapsibleLogEntry>();

        public LogViewModel()
        {
            
        }

        private ObservableCollection<CollapsibleLogEntry> LogEntries { get; set; }
          
        public void AddEntry(CollapsibleLogEntry entry)
        {
            _logEntries.Add(entry);
        }
        
        public int EventCount
        {
            get
            {
                return _logEntries.Count;
            }
        }
    }
}
