using Overlay;
using System;
using System.Net;
using System.Net.Sockets;

namespace Cache_Client
{
    public class Client : ICache
    {
        #region datamembers

        private Socket _sender;
        private Messenger _messenger;

        #endregion



        #region constructor

        public Client(IPEndPoint iPEndPoint, EventHandler<CustomEventArgs> handler)
        {
            //Creating a socket for client
            _sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _messenger = new Messenger(_sender, handler);
            RaiseEvent += handler;
            ConnectClient(iPEndPoint);

        }

        #endregion

        #region eventhandler
        public event EventHandler<CustomEventArgs> RaiseEvent;

        protected virtual void OnRaiseEvent(CustomEventArgs args)
        {
            if (RaiseEvent != null)
                RaiseEvent(this, args);
        }


        #endregion

        #region methods

        private void ConnectClient(IPEndPoint serverEndPoint)
        {

            try
            {
                //connecting client with server IPEndPoint
                _sender.Connect(serverEndPoint);

                OnRaiseEvent(new CustomEventArgs("My IP END POINT " + _sender.LocalEndPoint.ToString()));
                OnRaiseEvent(new CustomEventArgs("Connected to " + _sender.RemoteEndPoint.ToString()));

                //while(true)
                //{
                //    _messenger.ReceiveMessage();
                //}
            }

            catch (SocketException e)
            {
                OnRaiseEvent(new CustomEventArgs("Socket exception occured: Error while attempting to access the socket\n" + e.Message));

            }
            catch (InvalidOperationException e)
            {
                OnRaiseEvent(new CustomEventArgs("Invalid operation exception occured: \n" + e.Message));
            }

            catch (Exception e)
            {
                OnRaiseEvent(new CustomEventArgs(e.Message));
            }
        }

        #endregion


        #region Interface methods

        public void Add(string key, object value)
        {
            _messenger.SendMessage("Add", key, value);
        }

        public void Clear()
        {
            _messenger.SendMessage("Clear", null, null);
        }

        public void Dispose()
        {
            _messenger.SendMessage("Dispose", null, null);
            _sender.Shutdown(SocketShutdown.Both);
            _sender.Close();
        }

        public object Get(string key)
        {
            _messenger.SendMessage("Get", key, null);
            byte[] bytes = new byte[1024];
            return _messenger.ReceiveMessage(bytes);
        }

        public void Initialize()
        {
            _messenger.SendMessage("Initialize", null, null);
        }

        public void Remove(string key)
        {
            _messenger.SendMessage("Remove", key, null);
        }

        #endregion



    }
}
