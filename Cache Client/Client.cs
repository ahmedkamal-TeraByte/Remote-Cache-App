using Overlay;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Cache_Client
{
    public class Client : ICacheClient
    {
        #region datamembers

        private Socket _sender;
        private Messenger _messenger;
        private Thread thread;
        private bool _isRunning = false;

        private DataObject _getRequestData;
        private ManualResetEvent _resetEvent;
        private bool _isGracefulStop = false;
        #endregion



        #region constructor

        public Client(IPEndPoint iPEndPoint, EventHandler<CustomEventArgs> handler, EventHandler<DataObjectEventArgs> dataHandler)
        {
            //Creating a socket for client
            _sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _messenger = new Messenger(_sender, handler);
            _resetEvent = new ManualResetEvent(false);
            _getRequestData = null;
            RaiseEvent += handler;
            RaiseDataEvent += dataHandler;
            ConnectClient(iPEndPoint);

        }

        #endregion

        #region eventhandler
        private event EventHandler<CustomEventArgs> RaiseEvent;

        private event EventHandler<DataObjectEventArgs> RaiseDataEvent;

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

            catch (SocketException)
            {
                OnRaiseEvent(new CustomEventArgs("The Server is Not Running. Please Exit Now and Try Later \n"));
                //throw;

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
            _isRunning = true;
            while (_isRunning)
            {
                try
                {
                    //Thread.Sleep(10000);
                    data = _messenger.ReceiveMessage();

                    if (data.Identifier.Equals("Exception"))
                        throw (Exception)data.Value;
                    else if (data.Identifier.Equals("Get"))
                    {
                        _getRequestData = data;
                        _resetEvent.Set();
                    }
                    else
                        OnRaiseDataEvent(new DataObjectEventArgs(data));
                }
                catch (ArgumentException e)
                {
                    OnRaiseEvent(new CustomEventArgs("\n" + e.Message));
                    //throw e;
                }

                catch (Exception)
                {
                    if (!_isGracefulStop)
                    {
                        OnRaiseEvent(new CustomEventArgs("\nThe server is closed:::"));
                    }
                    break;
                }
            }
        }

        #endregion


        #region Interface methods

        public void Subscribe(string eventName)
        {
            _messenger.SendMessage("Subscribe", eventName, null);
        }

        public void Unsubscribe(string eventName)
        {
            _messenger.SendMessage("Unsubscribe", eventName, null);
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
            _isRunning = false;
            _isGracefulStop = true;
            _sender.Shutdown(SocketShutdown.Both);
            _sender.Close();
        }

        public object Get(string key)
        {

            _resetEvent.Reset();
            _messenger.SendMessage("Get", key, null);
            _resetEvent.WaitOne(2000);


            if (_getRequestData != null && _getRequestData.Key.Equals(key))
            {
                return _getRequestData;
            }

            else
            {
                return new DataObject()
                {
                    Identifier = "Get",
                    Key = key,
                    Value = "Try Later"
                };
            }


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
