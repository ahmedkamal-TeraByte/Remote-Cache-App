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
            Subscriptions.Add(registration.Key, registration);
        }

        public void UnSubscribe(string key)
        {
            Subscriptions.Remove(key);
        }

        public List<Registration> GetRegistrations(Socket socket)
        {
            List<Registration> registrations = new List<Registration>();
            foreach (var sub in Subscriptions)
            {
                if (sub.Value.Subscriber == socket)
                {
                    registrations.Add(sub.Value);
                }
            }

            return registrations;
        }

    }
}
