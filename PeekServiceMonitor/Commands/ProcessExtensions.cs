using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Management;
using log4net;
using System.Data;

namespace PeekServiceMonitor.Commands
{
    class ProcessExtensions
    {
        private readonly ILog logger;

        public ProcessExtensions()
        {
            logger = LogManager.GetLogger(typeof(ProcessExtensions));
        }

        public Process GetProcess(ServiceController svc)
        {
            if (svc.Status == ServiceControllerStatus.Stopped)
            {
                return null;    // stopped, so no process ID!
            }
                
            ManagementObject service = new ManagementObject(@"Win32_service.Name='" + svc.ServiceName + "'");
            object o = service.GetPropertyValue("ProcessId");
            int processId = (int)((UInt32)o);
            Process process = Process.GetProcessById(processId);
            
            return process;
        }

        public int GetProcessId(ServiceController svc)
        {
            if (svc.Status == ServiceControllerStatus.Stopped)
            {
                return 0;    // stopped, so no process ID!
            }

            ManagementObject service = new ManagementObject(@"Win32_service.Name='" + svc.ServiceName + "'");
            object o = service.GetPropertyValue("ProcessId");
            int processId = (int)((UInt32)o);
            Process process = Process.GetProcessById(processId);
            
            return processId;
        }

        public string GetStartTime(int processId)
        {
            String queryString = "SELECT CreationDate FROM Win32_Process WHERE ProcessId='" + processId + "'";

            string value = GetPropertyValue(queryString);

            return value;
        }

        public string GetStopTime(int processId)
        {
            String queryString = "SELECT TerminationDate FROM Win32_Process WHERE ProcessId='" + processId + "'";

            string value = GetPropertyValue(queryString);

            return value;
        }

        public void PrintAllPropertyValues(string svcName)
        {
            //String queryString = "SELECT * FROM Win32_Process WHERE Service='" + svcName + "'";
            //String queryString = "SELECT * FROM Win32_Process WHERE Description='StateMachineService.exe'";
            String queryString = "SELECT * FROM Win32_Process";

            SelectQuery query = new SelectQuery(queryString);

            ManagementScope scope = new ManagementScope(@"\\.\root\cimv2");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
            ManagementObjectCollection processes = searcher.Get();

            foreach (ManagementObject mo in processes)
            {
                PropertyDataCollection properties = mo.Properties;
            }
        }

        public void PrintAllPropertyValues(int processId)
        {
            String queryString = "SELECT * FROM Win32_Process WHERE ProcessId='" + processId + "'";

            SelectQuery query = new SelectQuery(queryString);

            ManagementScope scope = new ManagementScope(@"\\.\root\cimv2");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
            ManagementObjectCollection processes = searcher.Get();

            foreach (ManagementObject mo in processes)
            {
                PropertyDataCollection properties = mo.Properties;
            }
        }

        public string GetPropertyValue(string queryString)
        {
            SelectQuery query = new SelectQuery(queryString);

            ManagementScope scope = new ManagementScope(@"\\.\root\cimv2");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
            ManagementObjectCollection processes = searcher.Get();
            
            foreach (ManagementObject mo in processes)
            {
                PropertyDataCollection properties = mo.Properties;
                
                foreach (PropertyData property in properties)
                {
                    if (null != property.Value)
                    {
                        return ManagementDateTimeConverter.ToDateTime(property.Value.ToString()).ToString();
                    }
                }
            }
            
            return "N/A";
        }
    }
}
