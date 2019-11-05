using Overlay;
using System;
using System.IO;
using System.Net.Sockets;

namespace Cache_Server
{
    public class Messenger
    {
        private Socket _client;
        private Serializer _serializer;

        public Messenger()
        {
            _serializer = new Serializer();
        }
        public Messenger(Socket client)
        {
            _serializer = new Serializer();
            _client = client;
        }

        public DataObject Recieve()
        {

            byte[] bytesLength = new byte[4];
            byte[] bytes = new byte[1024];

            _client.Receive(bytesLength, 4, SocketFlags.None);
            _client.Receive(bytes, BitConverter.ToInt32(bytesLength, 0), SocketFlags.None);

            return _serializer.DeSerialize(new MemoryStream(bytes));
        }


        public void Send(DataObject data)
        {
            MemoryStream _stream = _serializer.Serialize(data);
            byte[] bytes = _stream.ToArray();
            byte[] byteslength = BitConverter.GetBytes(bytes.Length);
            try
            {
                _client.Send(byteslength);
                _client.Send(bytes);
            }
            catch (SocketException)
            {

                Console.WriteLine("The client socket has closed");
            }

        }

        public void SendNotification(Socket client, DataObject data)
        {
            try
            {
                MemoryStream _stream = _serializer.Serialize(data);
                byte[] bytes = _stream.ToArray();
                byte[] byteslength = BitConverter.GetBytes(bytes.Length);

                client.Send(byteslength);
                client.Send(bytes);

            }
            catch (Exception)
            {
                Console.WriteLine("Cannot send message because of serialization exception\n");
            }
        }
    }
}
