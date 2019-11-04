using Overlay;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Cache_Server
{
    public class Notifier
    {
        private EventsRegistry _eventsRegistry;
        public Notifier(EventsRegistry registry)
        {
            _eventsRegistry = registry;
        }


        public void Notify(string action, Notification notification)
        {
            Dictionary<string, Registration> registry = _eventsRegistry.GetRegistry();
            foreach (var reg in registry)
            {
                Registration registration = reg.Value;

                if (registration.Event == action)
                {

                    SendNotification(registration.Subscriber, notification);
                }
            }
        }

        private void SendNotification(Socket subscriber, Notification notification)
        {

            throw new NotImplementedException();
        }
    }
}
