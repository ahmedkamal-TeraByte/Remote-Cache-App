using Cache_Server;
using Overlay;
using System;
using System.Configuration;
using System.Net;
using System.ServiceProcess;

namespace CacheServerService
{
    public partial class Service1 : ServiceBase
    {
        private static Server _server;

        public Service1()
        {
            InitializeComponent();
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(ConfigurationManager.AppSettings["IPADDRESS"]), Convert.ToInt32(ConfigurationManager.AppSettings["Port"]));

            int maxCount = Convert.ToInt32(ConfigurationManager.AppSettings["MaxCacheCount"]);
            int time = Convert.ToInt32(ConfigurationManager.AppSettings["EvictionDuration"]);
            _server = new Server(iPEndPoint, HandleEvents, maxCount, time);

        }

        public void OnDebug()
        {
            OnStart(null);
        }

        protected override void OnStart(string[] args)
        {
            _server.StartServer();
        }

        protected override void OnStop()
        {
            _server.StopServer();
        }


        static void HandleEvents(object source, CustomEventArgs args)
        {
            Console.WriteLine(args.Message);
        }
    }
}
