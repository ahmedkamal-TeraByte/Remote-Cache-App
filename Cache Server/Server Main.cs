using Overlay;
using System;
using System.Configuration;
using System.Net;
using Topshelf;

namespace Cache_Server
{
    class ServerMain
    {
        private static Server _server;

        //public ServerMain(IPEndPoint iPEndPoint, int maxCount, int time)
        //{
        //    _server=new Server(iPEndPoint, HandleEvents, maxCount, time);
        //}

        static void Main(string[] args)
        {
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(ConfigurationManager.AppSettings["IPADDRESS"]), Convert.ToInt32(ConfigurationManager.AppSettings["Port"]));

            int maxCount = Convert.ToInt32(ConfigurationManager.AppSettings["MaxCacheCount"]);
            int time = Convert.ToInt32(ConfigurationManager.AppSettings["EvictionDuration"]);
            _server = new Server(iPEndPoint, HandleEvents, maxCount, time);

            StartServer();


            //var exitCode = HostFactory.Run(x =>
            // {
            //     x.Service<ServerMain>(s =>
            //     {
            //         s.ConstructUsing(server => new ServerMain(iPEndPoint, maxCount, time));
            //         s.WhenStarted(server => server.StartServer());
            //         s.WhenStopped(server => server.StopSever());
            //     });

            //     x.RunAsLocalSystem();

            //     x.SetServiceName("CacheServer");
            //     x.SetDisplayName("Cache Server");

            //     x.SetDescription("This is a sample Multithreaded server");
            // });

            //int exitCodeValue = (int)Convert.ChangeType(exitCode, exitCode.GetTypeCode());
            //Environment.ExitCode = exitCodeValue;


            ////new ServerMain(iPEndPoint);
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