using Overlay;
using System;
using System.Net.Sockets;

namespace Cache_Client
{
    public class Messenger
    {
        private Socket _socket;
        private Serializer _serializer;
        public Messenger(Socket socket, EventHandler<CustomEventArgs> handler)
        {
            _socket = socket;
            RaiseEvent += handler;
            _serializer = new Serializer(handler);
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
            byte[] bytes = _serializer.Serialize(new DataObject { Identifier = identifier, Key = key, Value = value });
            byte[] byteslength = BitConverter.GetBytes(bytes.Length);
            _socket.Send(byteslength);
            _socket.Send(bytes);
        }

        public DataObject ReceiveMessage()
        {

            byte[] bytesLength = new byte[4];
            _socket.Receive(bytesLength, 4, SocketFlags.None);
            byte[] bytes = new byte[BitConverter.ToInt32(bytesLength, 0)];
            _socket.Receive(bytes, BitConverter.ToInt32(bytesLength, 0), SocketFlags.None);
            return _serializer.DeSerialize(bytes);
        }
    }
}
