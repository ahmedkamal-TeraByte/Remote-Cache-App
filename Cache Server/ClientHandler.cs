using Overlay;
using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace Cache_Server
{
    class ClientHandler
    {
        private readonly ICache dataManager;
        private Socket _client;

        //creating a binaryformatter to serialize a data in stream
        private BinaryFormatter _formatter = new BinaryFormatter();

        //creating a stream to use for serialization
        private MemoryStream _stream;

        public ClientHandler(Socket client, ICache manager, EventHandler<CustomEventArgs> handler)
        {
            dataManager = manager;
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
                byte[] bytes = new byte[1024];
                byte[] bytesLength = new byte[4];
                try
                {
                    //recieves the bytes from socket
                    _client.Receive(bytesLength, 4, SocketFlags.None);
                    _client.Receive(bytes, BitConverter.ToInt32(bytesLength, 0), SocketFlags.None);

                    //convert the recieved bytes into stream to deseralize
                    _stream = new MemoryStream(bytes);
                    if (_stream != null)
                    {
                        data = DeSerialize(_stream);
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
            if (data.Identifier.Equals("Add"))
                dataManager.Add(data.Key, data.Value);
            else if (data.Identifier.Equals("Remove"))
                dataManager.Remove(data.Key);
            else if (data.Identifier.Equals("Get"))
            {
                object value = dataManager.Get(data.Key);
                Send(new DataObject
                {
                    Identifier = "Get",
                    Key = data.Key,
                    Value = value
                });
            }
            else if (data.Identifier.Equals("Clear"))
                dataManager.Clear();
            else if (data.Identifier.Equals("Dispose"))
            {
                dataManager.Dispose();
                OnRaiseEvent(new CustomEventArgs("The Client " + _client.RemoteEndPoint.ToString() + " was disconnected"));
                _client.Shutdown(SocketShutdown.Both);
                _client.Close();
            }
            else if (data.Identifier.Equals("Initialize"))
                dataManager.Initialize();
        }

        private void Send(DataObject data)
        {
            _stream = new MemoryStream();
            _stream = Serialize(data);
            byte[] bytes = _stream.ToArray();
            _client.Send(bytes);
        }

        private MemoryStream Serialize(DataObject data)
        {
            _formatter = new BinaryFormatter();
            _stream = new MemoryStream();
            try
            {
                _formatter.Serialize(_stream, data);
                return _stream;
            }
            catch (SerializationException e)
            {
                OnRaiseEvent(new CustomEventArgs("Cannot serialize Object at server " + e.Message));
                return null;
            }

        }

        private DataObject DeSerialize(Stream stream)
        {
            try
            {
                _formatter = new BinaryFormatter();
                DataObject data = (DataObject)_formatter.Deserialize(stream);
                return data;
            }
            catch (SerializationException e)
            {
                OnRaiseEvent(new CustomEventArgs("Cannot desearlize stream at server " + e.Message));
                throw;
                //return null;
            }

        }
    }
}
