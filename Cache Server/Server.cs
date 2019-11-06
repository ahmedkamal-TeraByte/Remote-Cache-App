using Overlay;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;


namespace Cache_Server
{
    class Server
    {
        private ICache _dataManager;
        private EventsRegistry _eventsRegistry;
        private IPEndPoint _iPEndPoint;
        private EventHandler<CustomEventArgs> _handler;
        private Socket _server;
        private List<ClientHandler> _clientHandlers;


        public Server(IPEndPoint iPEndPoint, EventHandler<CustomEventArgs> handler, int max, int time)
        {
            _dataManager = DataManager.GetInstance(handler, max, time);
            _clientHandlers = new List<ClientHandler>();
            _eventsRegistry = new EventsRegistry();
            _iPEndPoint = iPEndPoint;
            _handler = handler;
            RaiseEvent += handler;
            //StartServer(iPEndPoint, handler);
        }


        #region event handler
        public event EventHandler<CustomEventArgs> RaiseEvent;

        protected virtual void OnRaiseEvent(CustomEventArgs args)
        {
            if (RaiseEvent != null)
                RaiseEvent(this, args);
        }
        #endregion


        public void StartServer()
        {
            ///creating a socket
            _server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ///binding that socket with ipendPoint
            _server.Bind(_iPEndPoint);

            OnRaiseEvent(new CustomEventArgs("Server started at | " + _iPEndPoint.ToString()));
            try
            {
                //listen to the incoming connections
                _server.Listen(2);
                _dataManager.Initialize();
                OnRaiseEvent(new CustomEventArgs("Waiting for Incoming Connection......"));
                while (true)
                {
                    //accepts an incoming connection
                    Socket clientSocket = _server.Accept();

                    OnRaiseEvent(new CustomEventArgs(clientSocket.RemoteEndPoint.ToString() + " Connected\n"));
                    ClientHandler clientHandler = new ClientHandler(clientSocket, _dataManager, _handler, _eventsRegistry);
                    _clientHandlers.Add(clientHandler);
                }
            }
            catch (SocketException e)
            {
                OnRaiseEvent(new CustomEventArgs("Socket exception occured: Error while attempting to access the socket\n" + e.Message));
            }
            catch (ObjectDisposedException e)
            {
                OnRaiseEvent(new CustomEventArgs("Object disposed exception occured: The socket has been closed\n" + e.Message));
            }
            catch (InvalidOperationException e)
            {
                OnRaiseEvent(new CustomEventArgs("Invalid operation exception occured: \n" + e.Message));
            }

            catch (Exception e)
            {
                OnRaiseEvent(new CustomEventArgs(e.Message));
            }

            //finally
            //{
            //    if(_server.Connected)
            //        _server.Shutdown(SocketShutdown.Both);
            //    _server.Close();
            //}
        }

        public void StopServer()
        {
            try
            {
                //_server.Shutdown(SocketShutdown.Both);
                //_server.Disconnect(true);

                //_dataManager.Dispose();
                //_eventsRegistry.Dispose();

                foreach (var handler in _clientHandlers)
                {
                    handler.isEntertaining = false;
                }
                _server.Close();
                _server.Dispose();

            }
            catch (SocketException e)
            {

                OnRaiseEvent(new CustomEventArgs(e.Message));
            }
        }

    }
}