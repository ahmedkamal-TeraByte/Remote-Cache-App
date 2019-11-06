using Overlay;
using System;
using System.Configuration;
using System.Net;
using Topshelf;

namespace Cache_Server
{
    class ServerMain
    {
        private Server _server;
        public ServerMain(IPEndPoint iPEndPoint)
        {
            _server = new Server(iPEndPoint, HandleEvents);
        }

        static void Main(string[] args)
        {
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(ConfigurationManager.AppSettings["IPADDRESS"]), Convert.ToInt32(ConfigurationManager.AppSettings["Port"]));



            var exitCode = HostFactory.Run(x =>
             {
                 x.Service<ServerMain>(s =>
                 {
                     s.ConstructUsing(server => new ServerMain(iPEndPoint));
                     s.WhenStarted(server => server.StartServer());
                     s.WhenStopped(server => server.StopSever());
                 });

                 x.RunAsLocalSystem();

                 x.SetServiceName("CacheServer");
                 x.SetDisplayName("Cache Server");

                 x.SetDescription("This is a sample Multithreaded server");
             });

            int exitCodeValue = (int)Convert.ChangeType(exitCode, exitCode.GetTypeCode());
            Environment.ExitCode = exitCodeValue;


            //new ServerMain(iPEndPoint);
        }

        void HandleEvents(object source, CustomEventArgs args)
        {
            Console.WriteLine(args.Message);
        }


        void StartServer()
        {
            _server.StartServer();
        }

        void StopSever()
        {
            _server.StopServer();
        }
    }
}