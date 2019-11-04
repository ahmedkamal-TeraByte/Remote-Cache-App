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

        private EventsRegistry _eventsRegistry;


        public ClientHandler(Socket client, ICache manager, EventHandler<CustomEventArgs> handler, EventsRegistry eventsRegistry)
        {
            _messenger = new Messenger(client);
            dataManager = manager;
            _eventsRegistry = eventsRegistry;
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
            Thread thread = new Thread(Entertain);
            thread.Start();
        }

        //recieves the bytes from client, desearlize it and performs actions based on identifier
        private void Entertain()
        {
            DataObject data;
            while (true)
            {
                //byte[] bytes = new byte[1024];
                //byte[] bytesLength = new byte[4];
                try
                {
                    //recieves the bytes from socket
                    //_client.Receive(bytesLength, 4, SocketFlags.None);
                    //_client.Receive(bytes, BitConverter.ToInt32(bytesLength, 0), SocketFlags.None);

                    //convert the recieved bytes into stream to deseralize
                    //_stream = new MemoryStream(bytes);
                    //if (_stream != null)
                    {
                        //data = DeSerialize(_stream);
                        data = _messenger.Recieve();
                        if (data.Identifier.Equals("Dispose"))
                        {
                            PerformActions(data);
                            break;
                        }
                        PerformActions(data);
                    }
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

        //performs actions on DataObject based on identifier
        private void PerformActions(DataObject data)
        {

            string identifier = data.Identifier;

            switch (identifier)
            {
                case "Add":
                    dataManager.Add(data.Key, data.Value);

                    break;
                case "Remove":
                    dataManager.Remove(data.Key);
                    break;
                case "Get":
                    object value = dataManager.Get(data.Key);
                    _messenger.Send(new DataObject
                    {
                        Identifier = "Get",
                        Key = data.Key,
                        Value = value
                    });
                    break;
                case "Clear":
                    dataManager.Clear();
                    break;
                case "Dispose":
                    dataManager.Dispose();
                    OnRaiseEvent(new CustomEventArgs("The Client " + _client.RemoteEndPoint.ToString() + " was disconnected"));
                    _client.Shutdown(SocketShutdown.Both);
                    _client.Close();
                    break;
                case "Initialize":
                    dataManager.Initialize();
                    break;

            }


            //_eventsRegistry.Subscribe(new Registration("Add", _client));
            //_eventsRegistry.Subscribe(new Registration("Remove", _client));
        }



    }
}
