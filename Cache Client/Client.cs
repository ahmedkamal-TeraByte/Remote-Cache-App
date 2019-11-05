using Overlay;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Cache_Client
{
    public class Client
    {
        #region datamembers

        private Socket _sender;
        private Messenger _messenger;
        private Thread thread;

        #endregion



        #region constructor

        public Client(IPEndPoint iPEndPoint, EventHandler<CustomEventArgs> handler, EventHandler<DataObjectEventArgs> dataHandler)
        {
            //Creating a socket for client
            _sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _messenger = new Messenger(_sender, handler);
            RaiseEvent += handler;
            RaiseDataEvent += dataHandler;
            ConnectClient(iPEndPoint);

        }

        #endregion

        #region eventhandler
        public event EventHandler<CustomEventArgs> RaiseEvent;

        public event EventHandler<DataObjectEventArgs> RaiseDataEvent;

        protected virtual void OnRaiseEvent(CustomEventArgs args)
        {
            if (RaiseEvent != null)
                RaiseEvent(this, args);
        }

        protected virtual void OnRaiseDataEvent(DataObjectEventArgs args)
        {
            if (RaiseDataEvent != null)
                RaiseDataEvent(this, args);
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


                 thread = new Thread(StartListening);
                thread.Start();

            }

            catch (SocketException e)
            {
                OnRaiseEvent(new CustomEventArgs("Socket exception occured: Error while attempting to access the socket\n" + e.Message));
                throw;

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

        private void StartListening()
        {
            OnRaiseEvent(new CustomEventArgs("Listening for incoming data..."));
            DataObject data;
            while (true)
            {
                try
                {
                    data = _messenger.ReceiveMessage();

                    if (data.Identifier.Equals("Exception occured"))
                        throw (Exception)data.Value;

                    OnRaiseDataEvent(new DataObjectEventArgs(data));
                }
                catch (Exception)
                {
                    OnRaiseEvent(new CustomEventArgs("\nThe server is closed:::"));
                    break;
                }
            }
        }

        #endregion


        #region Interface methods

        public void Sub(string action)
        {
            _messenger.SendMessage("Subscribe", action, null);
        }

        public void UnSub(string action)
        {
            _messenger.SendMessage("Unsubscribe", action, null);
        }


        public void GetSubscriptions()
        {
            _messenger.SendMessage("Get Subscriptions", null, null);
        }

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
            thread.Abort();
            _sender.Shutdown(SocketShutdown.Both);
            _sender.Close();
        }

        public void Get(string key)
        {
            _messenger.SendMessage("Get", key, null);
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
