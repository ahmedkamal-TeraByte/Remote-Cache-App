using System;
using System.Configuration;
using System.Net;

namespace Cache_Server
{
    class ServerMain
    {

        public ServerMain(IPEndPoint iPEndPoint)
        {
            new Server(iPEndPoint, HandleEvents);
        }

        static void Main(string[] args)
        {
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(ConfigurationManager.AppSettings["IPADDRESS"]), Convert.ToInt32(ConfigurationManager.AppSettings["Port"]));

            new ServerMain(iPEndPoint);
        }

        void HandleEvents(object source, CustomEventArgs args)
        {
            Console.WriteLine(args.Message);
        }

    }
}