using Overlay;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Cache_Server
{
    public class Notifier
    {
        private EventsRegistry _eventsRegistry;
        private Messenger _messenger;
        public Notifier(EventsRegistry registry,EventHandler<CustomEventArgs> handler)
        {
            _eventsRegistry = registry;
            _messenger = new Messenger(handler);
        }


        public void Notify(string action, Notification notification)
        {
            Dictionary<string, Registration> registry = _eventsRegistry.GetRegistry();
            DataObject data = new DataObject();
            data.Identifier = "Notification";
            data.Key = null;
            data.Value = notification;
            foreach (var reg in registry)
            {
                Registration registration = reg.Value;

                if (registration.Event == action)
                {
                    _messenger.SendNotification(registration.Subscriber,data);                    
                }
            }
        }

    }
}
