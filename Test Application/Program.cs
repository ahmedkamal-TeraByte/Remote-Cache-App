using Cache_Client;
using Overlay;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Test_Application
{
    class StartMain
    {
        private static ICacheClient _client;
        private static bool completed = true;


        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            string portString = ConfigurationManager.AppSettings["Port"];
            if (Int32.TryParse(portString, out int port))
            {
                if (IPAddress.TryParse(ConfigurationManager.AppSettings["IPADDRESS"], out IPAddress iPAddress))
                {
                    IPEndPoint iPEndPoint = new IPEndPoint(iPAddress, port);
                    _client = new Client(iPEndPoint, HandleEvents, HandleDataEvents);
                    _client.Initialize();
                    StartClient();
                }
                else
                    Console.WriteLine("IP Address is not Valid");
            }
            else
                Console.WriteLine("Port Number is not Valid");

            Console.WriteLine("Press Enter to continue....");
            Console.Read();

            //int port = Convert.ToInt32(ConfigurationManager.AppSettings["Port"]);


            //IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(ConfigurationManager.AppSettings["IPADDRESS"]), port);
            //_client = new Client(iPEndPoint, HandleEvents, HandleDataEvents);
            //_client.Initialize();
            //StartClient();


        }


        #region methods

        private static void StartClient()
        {
            bool IsRunning = true;

            Thread thread = new Thread(KeepAdding);
            int choice;


            #region while
            while (IsRunning)
            {

                Thread.Sleep(1000);

                Console.WriteLine("\n+++ Main Menu +++:\n");

                Console.WriteLine("Enter 1 to ADD data");
                Console.WriteLine("Enter 2 to REMOVE data");
                Console.WriteLine("Enter 3 to GET data");
                Console.WriteLine("Enter 4 to CLEAR Cache");
                Console.WriteLine("Enter 5 to EXIT\n");
                Console.WriteLine("Enter 6 to Start a Thread");
                Console.WriteLine("Enter 7 to Subscribe for event");
                Console.WriteLine("Enter 8 to Unsubscribe from event");
                Console.WriteLine("Enter 9 to see list of Subscribed events");

                Console.Write("Enter Input:");

                string inputString = Console.ReadLine();


                if (Int32.TryParse(inputString, out int input))
                {
                    switch (input)
                    {
                        case 1:
                            Add();
                            break;
                        case 2:
                            Remove();
                            break;
                        case 3:
                            DataObject data = (DataObject)Get();
                            if (data != null && !data.Value.Equals("Try Later"))
                                Console.WriteLine("\n Key={0}\n Value={1}", data.Key, data.Value);
                            break;
                        case 4:
                            try
                            {
                                _client.Clear();
                            }
                            catch (SocketException e)
                            {
                                Console.WriteLine("The server is Closed." + e.Message);
                            }
                            break;
                        case 5:
                            try
                            {
                                _client.Dispose();
                            }
                            catch (SocketException e)
                            {
                                Console.WriteLine("The server is Closed." + e.Message);
                            }
                            IsRunning = false;
                            break;
                        case 6:
                            if (thread.ThreadState.Equals(ThreadState.Unstarted))
                            {
                                thread.Start(_client);
                                Console.WriteLine("Thread started");
                            }
                            else if (thread.ThreadState.Equals(ThreadState.WaitSleepJoin))
                            {
                                Console.Write("Thread is already running \nDo you want to stop this thread? \nPress 1 to stop this thread. Press 0 to go to main menu..");
                                try
                                {
                                    choice = Int32.Parse(Console.ReadLine());
                                }
                                catch (FormatException)
                                {
                                    Console.WriteLine("Invalid Choice");
                                    continue;
                                }

                                if (choice == 1)
                                {
                                    completed = false;
                                    Console.WriteLine("Thread stopped");
                                    break;
                                }
                                else if (choice == 0)
                                    break;
                                else
                                    Console.WriteLine("Invalid Choice");

                            }
                            else
                            {
                                completed = true;
                                Console.WriteLine("Thread state = {0}", thread.ThreadState);
                                thread = new Thread(KeepAdding);
                                thread.Start(_client);
                                Console.WriteLine("Thread started");

                            }
                            break;

                        case 7:
                            ShowSubMenu();
                            break;
                        case 8:
                            ShowUnsubMenu();
                            break;
                        default:
                            Console.WriteLine("Enter a number form shown menu\n Press anykey to continue...");
                            Console.ReadKey();
                            break;
                    }
                }
                else
                    Console.WriteLine("Please enter a number only\n");
            }

            #endregion

        }


        private static void KeepAdding(Object Cclient)
        {

            ICacheClient client = (ICacheClient)Cclient;
            int i = 0;
            while (completed)
            {
                Thread.Sleep(1000);
                TestObject obj = new TestObject(2000);
                //client.Add("key" + i, "value" + i);
                try
                {
                    client.Add("key" + i, obj);
                }
                catch (SocketException e)
                {
                    Console.WriteLine("The server is Closed." + e.Message);
                }
                i++;
            }

        }

        private static void Add()
        {
            string key;
            object value;
            Console.Write("\n Enter key:");
            key = Console.ReadLine();

            Console.Write("\n Enter Value:");
            value = Console.ReadLine();
            try
            {
                _client.Add(key, value);
            }
            catch (SocketException e)
            {
                Console.WriteLine("The server is Closed." + e.Message);
            }
        }

        private static void Remove()
        {
            string key;
            Console.Write("\n Enter key:");
            key = Console.ReadLine();
            try
            {
                _client.Remove(key);
            }
            catch (SocketException e)
            {
                Console.WriteLine("The server is Closed." + e.Message);
            }
        }

        private static object Get()
        {
            string key;
            Console.Write("\n Enter key:");
            key = Console.ReadLine();
            try
            {
                return _client.Get(key);
            }
            catch (SocketException e)
            {
                Console.WriteLine("The server is Closed." + e.Message);
                return null;
            }
        }

        private static void ShowSubMenu()
        {
            bool isOk = true;
            while (isOk)
            {
                Thread.Sleep(1000);

                Console.WriteLine("\n+++ Subscription Menu +++:\n");

                Console.WriteLine("Enter 1 to Subscribe for Add Event");
                Console.WriteLine("Enter 2 to Subscribe for Remove Event");
                Console.WriteLine("Enter 3 to Subscribe for Cache Clear Event");
                Console.WriteLine("Enter 0 to Go back to Main menu");

                Console.Write("Enter Input:");
                string inputString = Console.ReadLine();


                if (Int32.TryParse(inputString, out int input))
                {
                    try
                    {
                        switch (input)
                        {
                            case 1:
                                _client.Subscribe("Add");
                                break;
                            case 2:
                                _client.Subscribe("Remove");
                                break;
                            case 3:
                                _client.Subscribe("Clear");
                                break;

                            case 0:
                                isOk = false;
                                break;

                            default:
                                Console.WriteLine("\n Please enter a number from show menu only:\n");
                                break;
                        }
                    }

                    catch (SocketException e)
                    {
                        Console.WriteLine("The server is Closed." + e.Message);

                    }
                }

                else
                    Console.WriteLine("Enter a valid Number");
            }
        }

        private static void ShowUnsubMenu()
        {
            ShowSubscriptions();
            bool isOk = true;
            while (isOk)
            {

                Thread.Sleep(1000);
                Console.WriteLine("\n+++ Unsubscription Menu +++:\n");

                Console.WriteLine("Enter 1 to UnSubscribe from Add Event");
                Console.WriteLine("Enter 2 to UnSubscribe from Remove Event");
                Console.WriteLine("Enter 3 to UnSubscribe from Cache Clear Event");
                Console.WriteLine("Enter 0 to Go back to Main menu");

                Console.Write("Enter Input:");
                string inputString = Console.ReadLine();


                if (Int32.TryParse(inputString, out int input))
                {
                    switch (input)
                    {
                        case 1:
                            _client.Unsubscribe("Add");
                            break;
                        case 2:
                            _client.Unsubscribe("Remove");
                            break;
                        case 3:
                            _client.Unsubscribe("Clear");
                            break;

                        case 0:
                            isOk = false;
                            break;

                        default:
                            Console.WriteLine("\n Please enter a number from shown menu only:\n");
                            break;
                    }
                }

                else
                    Console.WriteLine("Enter a valid Number");

            }
        }

        private static void ShowSubscriptions()
        {
            _client.GetSubscriptions();
        }

        #endregion

        #region eventhandlers
        private static void HandleEvents(object sender, CustomEventArgs args)
        {
            Console.WriteLine(args.Message);
        }

        private static void HandleDataEvents(object sender, DataObjectEventArgs args)
        {
            DataObject data = args.dataObject;

            string identifier = data.Identifier;

            switch (identifier)
            {
                case "Get":
                    Console.WriteLine("\n Key={0}\n Value={1}", data.Key, data.Value);
                    break;

                case "Notification":
                    Notification notification = (Notification)data.Value;

                    if (notification.Key != null && notification.Value != null)
                        Console.WriteLine("\n NOTIFICATION\n Key={0} \n Value={1} \t {2}", notification.Key, notification.Value, notification.Message);
                    else if (notification.Value == null)
                        Console.WriteLine("\n NOTIFICATION\n Key={0} \t {1}", notification.Key, notification.Message);
                    else
                        Console.WriteLine("\n NOTIFICATION\n {0}", notification.Message);

                    break;

                case "Subscriptions":
                    var list = (List<string>)data.Value;
                    if (list.Count > 0)
                    {
                        Console.WriteLine("\nYou have following subscriptions:");
                        foreach (var sub in list)
                        {
                            Console.WriteLine(sub);
                        }
                    }
                    else
                        Console.WriteLine("You don't have any subscriptions yet");
                    break;

                case "Exception occured":
                    Exception e = (Exception)data.Value;
                    Console.WriteLine(e.Message);
                    break;
            }
        }

        #endregion
    }
}
