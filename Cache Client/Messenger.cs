using Overlay;
using System;
using System.IO;
using System.Net.Sockets;

namespace Cache_Client
{
    public class Messenger
    {
        private Socket _socket;
        private Serializer _serializer;
        private MemoryStream _stream;
        public Messenger(Socket socket, EventHandler<CustomEventArgs> handler)
        {
            _socket = socket;
            RaiseEvent += handler;
            _serializer = new Serializer(handler);
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
            _stream.Flush();
            _stream = _serializer.Serialize(new DataObject { Identifier = identifier, Key = key, Value = value });
            byte[] bytes = _stream.ToArray();

            byte[] byteslength = BitConverter.GetBytes(bytes.Length);
            try
            {
                _socket.Send(byteslength);
                _socket.Send(bytes);
            }
            catch (SocketException)
            {
                OnRaiseEvent(new CustomEventArgs("The server is NOT OPEN"));
            }
        }

        public DataObject ReceiveMessage()
        {
            byte[] bytes = new byte[1024];
            byte[] bytesLength = new byte[4];

           
            try
            {
                _socket.Receive(bytesLength, 4, SocketFlags.None);
                _socket.Receive(bytes, BitConverter.ToInt32(bytesLength, 0), SocketFlags.None);
                return _serializer.DeSerialize(new MemoryStream(bytes));

            }
            catch (ObjectDisposedException e)
            {
                //OnRaiseEvent(new CustomEventArgs("The server socket is CLOSED"));
                return new DataObject { Identifier = "Exception occured", Key = null, Value = e };
            }
            catch (Exception e)
            {
                //OnRaiseEvent(new CustomEventArgs(e.Message));
                return new DataObject { Identifier = "Exception occured", Key = null, Value = e };
            }

        }



    }
}
