using Overlay;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Cache_Server
{
    public class EventsRegistry
    {

        private Dictionary<string, Registration> Subscriptions = null;


        public EventsRegistry()
        {
            Subscriptions = new Dictionary<string, Registration>();
        }


        public Dictionary<string, Registration> GetRegistry()
        {
            return Subscriptions;
        }

        public void Subscribe(Registration registration)
        {
            //Console.WriteLine("subscribing " + registration.Subscriber.RemoteEndPoint + " for " + registration.Event + "event \n");

            if (!Subscriptions.TryGetValue(registration.Key, out _))
                Subscriptions.Add(registration.Key, registration);
        }

        public void UnSubscribe(Socket client, string action)
        {
            //Console.WriteLine("unsubscribing " + client.RemoteEndPoint + " from " + action + "event \n");
            string key = action + client.RemoteEndPoint.ToString();
            if (Subscriptions.TryGetValue(key, out _))
                Subscriptions.Remove(key);
        }

        public List<string> GetMyRegistrations(Socket socket)
        {
            List<string> registrations = new List<string>();
            foreach (var sub in Subscriptions)
            {
                if (sub.Value.Subscriber == socket)
                {
                    registrations.Add(sub.Value.Event);
                }
            }

            return registrations;
        }

    }
}
