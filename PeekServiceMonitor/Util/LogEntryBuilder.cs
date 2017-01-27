using log4net;
using PeekServiceMonitor.View;
using PeekServiceMonitor.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PeekServiceMonitor.Util
{
    public class LogEntryBuilder
    {
        private readonly ILog logger;
        private static int index;

        public LogEntryBuilder()
        {
            logger = LogManager.GetLogger(typeof(LogEntryBuilder));
        }

        public void CaptureEvents()
        {
            List<String> eventList = new List<string>();
            string query = "*[System/Level=1 or System/Level=2]";
            EventLogQuery eventQuery = new EventLogQuery("Application", PathType.LogName, query);

            LogViewModel logViewModel = new LogViewModel();

            try
            {
                EventLogReader logReader = new EventLogReader(eventQuery);

                index = 0;
                for (EventRecord eventdetail = logReader.ReadEvent(); eventdetail != null; eventdetail = logReader.ReadEvent())
                {
                    var msg = eventdetail.FormatDescription();

                    //If query returns 0 matches - skip Regex
                    if (msg == null)
                    {
                        break;
                    }

                    if (Regex.IsMatch(msg, "peek", RegexOptions.IgnoreCase)
                        || Regex.IsMatch(msg, "semex", RegexOptions.IgnoreCase)
                        || Regex.IsMatch(msg, "spinnaker", RegexOptions.IgnoreCase))
                    {
                        logViewModel.AddEntry(CreateLogEntry(eventdetail, index));
                    }
                }
            }
            catch (EventLogNotFoundException ex)
            {
                Console.WriteLine($"Error while reading the event logs: {ex}");
            }
        }

        public CollapsibleLogEntry CreateLogEntry(EventRecord record, int index)
        {
            string desc = record.FormatDescription();
            string name = desc.Substring(0, 80) + "...";
            
            return new CollapsibleLogEntry
            {
                Index = index++,
                DateTime = (DateTime)record.TimeCreated,
                Message = name,
                Contents = new List<LogEntry>
                {
                    new LogEntry
                    {
                        Index = index++,
                        DateTime = (DateTime)record.TimeCreated,
                        Message = desc
                    }
                }
            };
        }
    }
}
