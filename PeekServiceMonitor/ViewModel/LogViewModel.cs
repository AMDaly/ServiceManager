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
        private static ObservableCollection<CollapsibleLogEntry> LogEntries { get; set; }
        public static ObservableCollection<CollapsibleLogEntry> _logEntries = new ObservableCollection<CollapsibleLogEntry>();

        public LogViewModel()
        {
            
        }

        public void AddEntry(CollapsibleLogEntry entry/*int index, DateTime dateTime, string message*/)
        {
/*
            CollapsibleLogEntry entry = new CollapsibleLogEntry
            {
                Index = index,
                DateTime = DateTime.UtcNow,
                Message = message,
                Contents = new List<LogEntry> {
                                new LogEntry
                                {
                                    Index = index,
                                    DateTime = DateTime.UtcNow,
                                    Message = message
                                },
                                new LogEntry
                                {
                                    Index = index,
                                    DateTime = DateTime.UtcNow,
                                    Message = message
                                }
                            }
            };
*/
            _logEntries.Add(entry);
        }
    }
}
