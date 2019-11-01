using Overlay;
using System;
using System.IO;
using System.Net.Sockets;

namespace Cache_Client
{
    class Messenger
    {
        private Socket _socket;
        private Serializer serializer;
        private MemoryStream _stream;
        public Messenger(Socket socket, EventHandler<CustomEventArgs> handler)
        {
            _socket = socket;
            RaiseEvent += handler;
            serializer = new Serializer(handler);
            _stream = new MemoryStream();
        }

        #region eventhandler
        public event EventHandler<CustomEventArgs> RaiseEvent;

        protected virtual void OnRaiseEvent(CustomEventArgs args)
        {
            if (RaiseEvent != null)
                RaiseEvent(this, args);
        }


        #endregion

        public void SendMessage(string identifier, string key, object value)
        {
            _stream = serializer.Serialize(new DataObject { Identifier = identifier, Key = key, Value = value });
            byte[] bytes = _stream.ToArray();

            byte[] byteslength = BitConverter.GetBytes(bytes.Length);
            try
            {
                //OnRaiseEvent(new CustomEventArgs("Sending message in messenger"));
                _socket.Send(byteslength);
                _socket.Send(bytes);
            }
            catch (SocketException)
            {
                OnRaiseEvent(new CustomEventArgs("The server is NOT OPEN"));
            }
        }

        public DataObject ReceiveMessage(byte[] bytes)
        {
            try
            {
                _socket.Receive(bytes);
                return serializer.DeSerialize(new MemoryStream(bytes));

            }
            catch (ObjectDisposedException)
            {
                OnRaiseEvent(new CustomEventArgs("The server socket is CLOSED"));
                return new DataObject { Identifier = "Exception occured", Key = null, Value = null };
            }
            catch (Exception e)
            {
                OnRaiseEvent(new CustomEventArgs(e.Message));
                return new DataObject { Identifier = "Exception occured", Key = null, Value = null };
            }

        }
    }
}
