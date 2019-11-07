using Cache_Server;
using Overlay;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.ServiceProcess;

namespace CacheServerService
{
    public partial class CacheServerService : ServiceBase
    {
        private static Server _server;

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(System.IntPtr handle, ref ServiceStatus serviceStatus);

        public CacheServerService()
        {
            Debugger.Launch();

            InitializeComponent();



            //initializing event log
            eventLog = new EventLog();
            if (!EventLog.SourceExists("MySource"))
            {
                EventLog.CreateEventSource(
                    "MySource", "MyNewLog");
            }
            eventLog.Source = "MySource";
            eventLog.Log = "MyNewLog";

            WriteToFile("Log created: ");

            //getting values from APP.CONFIG and creating server

            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(ConfigurationManager.AppSettings["IPADDRESS"]), Convert.ToInt32(ConfigurationManager.AppSettings["Port"]));

            int maxCount = Convert.ToInt32(ConfigurationManager.AppSettings["MaxCacheCount"]);
            int time = Convert.ToInt32(ConfigurationManager.AppSettings["EvictionDuration"]);

            //starting server
            _server = new Server(iPEndPoint, HandleEvents, maxCount, time);

        }



        public enum ServiceState
        {
            SERVICE_STOPPED = 0x00000001,
            SERVICE_START_PENDING = 0x00000002,
            SERVICE_STOP_PENDING = 0x00000003,
            SERVICE_RUNNING = 0x00000004,
            SERVICE_CONTINUE_PENDING = 0x00000005,
            SERVICE_PAUSE_PENDING = 0x00000006,
            SERVICE_PAUSED = 0x00000007,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ServiceStatus
        {
            public int dwServiceType;
            public ServiceState dwCurrentState;
            public int dwControlsAccepted;
            public int dwWin32ExitCode;
            public int dwServiceSpecificExitCode;
            public int dwCheckPoint;
            public int dwWaitHint;
        };






        public void OnDebug()
        {
            OnStart(null);
        }

        protected override void OnStart(string[] args)
        {



            //Debugger.Launch();

            eventLog.WriteEntry("In OnStart.");


            // Update the service state to Start Pending.
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            new System.Threading.Thread(() =>
            {
                _server.StartServer();
            }).Start();

            WriteToFile("\nServer started at " + DateTime.Now);

            // Update the service state to Running.
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }

        protected override void OnStop()
        {
            //_server.StopServer();

            eventLog.WriteEntry("In OnStop.");

            // Update the service state to Stop Pending.
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOP_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);


            _server.StopServer();

            WriteToFile("\nService Stopped at " + DateTime.Now);

            // Update the service state to Stopped.
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOPPED;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }

        private void WriteToFile(string Message)
        {
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
                if (!File.Exists(filepath))
                {
                    // Create a file to write to.   
                    using (StreamWriter sw = File.CreateText(filepath))
                    {
                        sw.WriteLine(Message);
                    }
                }
                else
                {
                    using (StreamWriter sw = File.AppendText(filepath))
                    {
                        sw.WriteLine(Message);
                    }
                }
            }
            catch (IOException)
            {
            }
        }


        void HandleEvents(object source, CustomEventArgs args)
        {
            WriteToFile(args.Message);
            eventLog.WriteEntry(args.Message);
        }
    }
}
