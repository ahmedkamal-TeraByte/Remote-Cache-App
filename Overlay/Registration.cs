using System.Net.Sockets;
using System;
namespace Overlay
{
    [Serializable]
    public class Registration
    {

        public Registration(string eve, Socket sub)
        {
            Event = eve;
            Subscriber = sub;

            Key = eve + sub.RemoteEndPoint.ToString();
        }

        public string Key { get; set; }
        public string Event { get; set; }
        public Socket Subscriber { get; set; }
    }
}
