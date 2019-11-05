using Overlay;
using System;
using System.Net;
using System.Net.Sockets;


namespace Cache_Server
{
    class Server
    {
        private ICache _dataManager;
        private EventsRegistry eventsRegistry;
        public Server(IPEndPoint iPEndPoint, EventHandler<CustomEventArgs> handler)
        {
            _dataManager = DataManager.GetInstance(handler);

            eventsRegistry = new EventsRegistry();
            RaiseEvent += handler;
            StartServer(iPEndPoint, handler);
        }


        #region event handler
        public event EventHandler<CustomEventArgs> RaiseEvent;

        protected virtual void OnRaiseEvent(CustomEventArgs args)
        {
            if (RaiseEvent != null)
                RaiseEvent(this, args);
        }
        #endregion


        private void StartServer(IPEndPoint iPEndPoint, EventHandler<CustomEventArgs> handler)
        {
            ///creating a socket
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ///binding that socket with ipendPoint
            serverSocket.Bind(iPEndPoint);

            OnRaiseEvent(new CustomEventArgs("Server started at | " + iPEndPoint.ToString()));
            try
            {
                //listen to the incoming connections
                serverSocket.Listen(2);
                DataManager dataManager = DataManager.GetInstance(handler);
                dataManager.Initialize();
                OnRaiseEvent(new CustomEventArgs("Waiting for Incoming Connection......"));
                while (true)
                {
                    //accepts an incoming connection
                    Socket clientSocket = serverSocket.Accept();

                    OnRaiseEvent(new CustomEventArgs(clientSocket.RemoteEndPoint.ToString() + " Connected\n"));
                    ClientHandler clientHandler = new ClientHandler(clientSocket, _dataManager, handler, eventsRegistry);
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

            finally
            {

                //clientSocket.Shutdown(SocketShutdown.Both);
                //clientSocket.Close();
                serverSocket.Shutdown(SocketShutdown.Both);
                serverSocket.Close();
            }
        }

    }
}