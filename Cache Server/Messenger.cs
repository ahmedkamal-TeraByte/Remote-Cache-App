using Overlay;
using System;
using System.Net.Sockets;

namespace Cache_Server
{
    public class Messenger
    {
        private Socket _client;
        private Serializer _serializer;

        public Messenger(EventHandler<CustomEventArgs> handler)
        {
            _serializer = new Serializer(handler);
            RaiseEvent += handler;
        }
        public Messenger(Socket client, EventHandler<CustomEventArgs> handler)
        {
            _serializer = new Serializer(handler);
            RaiseEvent += handler;
            _client = client;
        }



        #region event handler
        public event EventHandler<CustomEventArgs> RaiseEvent;

        protected virtual void OnRaiseEvent(CustomEventArgs args)
        {
            if (RaiseEvent != null)
                RaiseEvent(this, args);
        }
        #endregion

        public DataObject Recieve()
        {

            byte[] bytesLength = new byte[4];
            _client.Receive(bytesLength, 4, SocketFlags.None);
            byte[] bytes = new byte[BitConverter.ToInt32(bytesLength, 0)];
            _client.Receive(bytes, BitConverter.ToInt32(bytesLength, 0), SocketFlags.None);

            return _serializer.DeSerialize(bytes);
        }


        public void Send(DataObject data)
        {

            byte[] bytes = _serializer.Serialize(data);
            byte[] byteslength = BitConverter.GetBytes(bytes.Length);
            try
            {
                _client.Send(byteslength);
                _client.Send(bytes);
            }
            catch (SocketException)
            {

                OnRaiseEvent(new CustomEventArgs("The client socket has closed"));
            }

        }

        public void SendNotification(Socket client, DataObject data)
        {
            try
            {
                byte[] bytes = _serializer.Serialize(data);
                byte[] byteslength = BitConverter.GetBytes(bytes.Length);

                client.Send(byteslength);
                client.Send(bytes);
                //OnRaiseEvent(new CustomEventArgs("Sending notification to" + client.RemoteEndPoint.ToString()));


            }
            catch (ObjectDisposedException e)
            {
                OnRaiseEvent(new CustomEventArgs("The subscriber is not connected: " + e.Message));
            }
        }
    }
}
