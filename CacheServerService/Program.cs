using System.ServiceProcess;

namespace CacheServerService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {

            //CacheServerService service1 = new CacheServerService();
            //service1.OnDebug();
            //System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new CacheServerService()
            };
            ServiceBase.Run(ServicesToRun);

        }
    }
}
