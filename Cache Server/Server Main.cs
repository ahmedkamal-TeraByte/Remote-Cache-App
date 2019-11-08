using Overlay;
using System;
using System.Configuration;
using System.Net;

namespace Cache_Server
{
    class ServerMain
    {
        private static Server _server;

        static void Main(string[] args)
        {
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(ConfigurationManager.AppSettings["IPADDRESS"]), Convert.ToInt32(ConfigurationManager.AppSettings["Port"]));

            int maxCount = Convert.ToInt32(ConfigurationManager.AppSettings["MaxCacheCount"]);
            int time = Convert.ToInt32(ConfigurationManager.AppSettings["EvictionDuration"]);
            _server = new Server(iPEndPoint, HandleEvents, maxCount, time);

            StartServer();

        }

        static void HandleEvents(object source, CustomEventArgs args)
        {
            Console.WriteLine(args.Message);
        }


        static void StartServer()
        {
            _server.StartServer();
        }

        void StopSever()
        {
            _server.StopServer();
        }
    }
}