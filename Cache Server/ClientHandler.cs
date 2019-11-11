using Overlay;
using System;
using System.Net.Sockets;
using System.Threading;

namespace Cache_Server
{
    class ClientHandler
    {
        private readonly ICache dataManager;
        private Socket _client;
        private Messenger _messenger;
        private Notifier _notifier;
        private EventsRegistry _eventsRegistry;
        public bool isEntertaining { get; set; } = false;
        Thread thread;


        public ClientHandler(Socket client, ICache manager, EventHandler<CustomEventArgs> handler, EventsRegistry eventsRegistry)
        {
            _messenger = new Messenger(client, handler);
            dataManager = manager;
            _eventsRegistry = eventsRegistry;
            _notifier = new Notifier(eventsRegistry, handler);
            thread = new Thread(Entertain);
            RaiseEvent += handler;
            HandleClient(client);
        }


        #region event handler
        public event EventHandler<CustomEventArgs> RaiseEvent;

        protected virtual void OnRaiseEvent(CustomEventArgs args)
        {
            if (RaiseEvent != null)
                RaiseEvent(this, args);
        }
        #endregion


        //recieves a client and create a new thread for each client
        private void HandleClient(Socket client)
        {
            _client = client;

            thread.Start();
        }

        //recieves the bytes from client, desearlize it and performs actions based on identifier
        private void Entertain()
        {
            DataObject data;
            isEntertaining = true;
            while (isEntertaining)
            {

                try
                {
                    data = _messenger.Recieve();
                    if (data.Identifier.Equals("Dispose"))
                    {
                        PerformActions(data);
                        break;
                    }
                    PerformActions(data);
                }
                catch (SocketException)
                {
                    OnRaiseEvent(new CustomEventArgs("The Client " + _client.RemoteEndPoint.ToString() + " was disconnected"));
                    _client.Shutdown(SocketShutdown.Both);
                    _client.Close();
                    break;
                }

            }
        }

        public void Abort()
        {
            thread.Abort();
        }


        //performs actions on DataObject based on identifier
        private void PerformActions(DataObject data)
        {

            string identifier = data.Identifier;

            //Exception e = new ArgumentException()

            switch (identifier)
            {
                case "Add":
                    try
                    {
                        dataManager.Add(data.Key, data.Value);
                        _notifier.Notify("Add", new Notification(data.Key, data.Value, "\t Data has been added...\n"));
                    }
                    catch (Exception e)
                    {
                        _messenger.Send(new DataObject
                        {
                            Identifier = "Exception",
                            Key = data.Key,
                            Value = e
                        });
                        //throw;
                    }
                    break;
                case "Remove":

                    try
                    {
                        dataManager.Remove(data.Key);
                        _notifier.Notify("Remove", new Notification(data.Key, null, "\t Data has been removed...\n"));
                    }
                    catch (Exception e)
                    {

                        _messenger.Send(new DataObject
                        {
                            Identifier = "Exception",
                            Key = data.Key,
                            Value = e
                        });
                    }
                    break;
                case "Get":

                    //OnRaiseEvent(new CustomEventArgs("get request recieved for key :" + data.Key + " "));
                    try
                    {
                        object value = dataManager.Get(data.Key);
                        _messenger.Send(new DataObject
                        {
                            Identifier = "Get",
                            Key = data.Key,
                            Value = value
                        });
                    }
                    catch (Exception e)
                    {
                        _messenger.Send(new DataObject
                        {
                            Identifier = "Exception",
                            Key = data.Key,
                            Value = e
                        });
                    }
                    break;
                case "Clear":
                    dataManager.Clear();
                    _notifier.Notify("Clear", new Notification(null, null, "The cache has been cleared"));
                    break;
                case "Dispose":
                    OnRaiseEvent(new CustomEventArgs("The Client " + _client.RemoteEndPoint.ToString() + " was disconnected"));
                    _eventsRegistry.UnsubscribeAllEvents(_client);
                    _client.Shutdown(SocketShutdown.Both);
                    _client.Close();
                    break;
                case "Initialize":
                    dataManager.Initialize();
                    break;

                case "Subscribe":
                    _eventsRegistry.Subscribe(new Registration(data.Key, _client));
                    break;
                case "Unsubscribe":
                    _eventsRegistry.UnSubscribe(_client, data.Key);
                    break;

                case "Get Subscriptions":
                    var list = _eventsRegistry.GetMyRegistrations(_client);

                    _messenger.Send(new DataObject
                    {
                        Identifier = "Subscriptions",
                        Key = null,
                        Value = list
                    });
                    break;
            }
        }
    }
}
