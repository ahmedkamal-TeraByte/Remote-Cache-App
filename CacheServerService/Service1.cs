using Cache_Server;
using Overlay;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.ServiceProcess;

namespace CacheServerService
{
    public partial class CacheServerService : ServiceBase
    {
        private static Server _server;

        public CacheServerService()
        {
            InitializeComponent();
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(ConfigurationManager.AppSettings["IPADDRESS"]), Convert.ToInt32(ConfigurationManager.AppSettings["Port"]));

            int maxCount = Convert.ToInt32(ConfigurationManager.AppSettings["MaxCacheCount"]);
            int time = Convert.ToInt32(ConfigurationManager.AppSettings["EvictionDuration"]);
            _server = new Server(iPEndPoint, HandleEvents, maxCount, time);

        }

        //public void OnDebug()
        //{
        //    OnStart(null);
        //}

        protected override void OnStart(string[] args)
        {
            _server.StartServer();
        }

        protected override void OnStop()
        {
            _server.StopServer();
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
        }
    }
}
