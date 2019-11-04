using System.Net.Sockets;

namespace Cache_Server
{
    public class Registration
    {

        public Registration(string eve, Socket sub)
        {
            Event = eve;
            Subscriber = sub;

            Key = eve + sub.LocalEndPoint.ToString();
        }

        public string Key { get; set; }
        public string Event { get; set; }
        public Socket Subscriber { get; set; }
    }
}
