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
            if (Int32.TryParse(ConfigurationManager.AppSettings["Port"], out int port) && IPAddress.TryParse(ConfigurationManager.AppSettings["IPADDRESS"], out IPAddress iPAddress))
            {


                //use second ipEndPoint for creating server on available IPADDRESSES like localhost and current server IP
                IPEndPoint iPEndPoint = new IPEndPoint(iPAddress, port);
                //IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, port);

                if (Int32.TryParse(ConfigurationManager.AppSettings["MaxCacheCount"], out int maxCount) && Int32.TryParse(ConfigurationManager.AppSettings["EvictionDuration"], out int time))
                {
                    _server = new Server(iPEndPoint, HandleEvents, maxCount, time);
                    StartServer();
                }
                else
                    Console.WriteLine("Please enter a valid MAX COUNT or EVICTION DURATION in config File");
            }
            else
                Console.WriteLine("Please enter a valid IP ADDRESS or PORT NUMBER in app.config file");
            Console.WriteLine("Press Enter to continue....");
            Console.Read();



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