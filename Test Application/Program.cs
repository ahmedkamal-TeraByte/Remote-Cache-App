using Cache_Client;
using Overlay;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Threading;

namespace Test_Application
{
    class StartMain
    {
        private Client _client;
        private bool completed = true;

        public StartMain(IPEndPoint iPEndPoint)
        {

            _client = new Client(iPEndPoint, HandleEvents, HandleDataEvents);
            _client.Initialize();


        }



        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            int port = Convert.ToInt32(ConfigurationManager.AppSettings["Port"]);

            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(ConfigurationManager.AppSettings["IPADDRESS"]), port);
            StartMain main = new StartMain(iPEndPoint);
            main.StartClient();


        }


        #region methods

        private void StartClient()
        {
            bool IsRunning = true;
            //Thread thread = new Thread(() =>
            //{
            //    KeepAdding(client);
            //});
            Thread thread = new Thread(KeepAdding);

            //thread.Start(_client);

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
                            Get();
                            break;
                        case 4:
                            _client.Clear();
                            break;
                        case 5:
                            _client.Dispose();
                            IsRunning = false;
                            break;
                        case 6:
                            if (thread.ThreadState.Equals(ThreadState.Unstarted))
                                thread.Start(_client);
                            else
                            {
                                completed = true;
                                Console.WriteLine("Thread state = {0}",thread.ThreadState);
                                thread = new Thread(KeepAdding);
                                thread.Start(_client);
                            }
                            Console.WriteLine("Thread started");
                            while (true)
                            {
                                Console.WriteLine("Press 0 to stop thread");
                                try
                                {
                                    choice = Int32.Parse(Console.ReadLine());
                                }
                                catch (FormatException)
                                {
                                    continue;
                                }
                                while (true)
                                {
                                    if (choice == 0 )
                                    {
                                        completed = false;
                                        break;
                                    }

                                }

                                break;
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


        private void KeepAdding(Object Cclient)
        {

            Client client = (Client)Cclient;
            int i = 0;
            while (completed)
            {
                Thread.Sleep(1000);
                client.Add("key" + i, "value" + i);
                i++;
            }
           
        }

        private void Add()
        {
            string key;
            object value;
            Console.Write("\n Enter key:");
            key = Console.ReadLine();

            Console.Write("\n Enter Value:");
            value = Console.ReadLine();
            _client.Add(key, value);
        }

        private void Remove()
        {
            string key;
            Console.Write("\n Enter key:");
            key = Console.ReadLine();
            _client.Remove(key);
        }

        private void Get()
        {
            string key;
            Console.Write("\n Enter key:");
            key = Console.ReadLine();
            _client.Get(key);
        }

        private void ShowSubMenu()
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
                    switch (input)
                    {
                        case 1:
                            _client.Sub("Add");
                            break;
                        case 2:
                            _client.Sub("Remove");
                            break;
                        case 3:
                            _client.Sub("Clear");
                            break;

                        case 0:
                            isOk = false;
                            break;

                        default:
                            Console.WriteLine("\n Please enter a number from show menu only:\n");
                            break;
                    }
                }

                else
                    Console.WriteLine("Enter a valid Number");
            }
        }

        private void ShowUnsubMenu()
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
                            _client.UnSub("Add");
                            break;
                        case 2:
                            _client.UnSub("Remove");
                            break;
                        case 3:
                            _client.UnSub("Clear");
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

        private void ShowSubscriptions()
        {
            _client.GetSubscriptions();

        }

        #endregion

        #region eventhandlers
        void HandleEvents(object sender, CustomEventArgs args)
        {
            Console.WriteLine(args.Message);
        }

        private void HandleDataEvents(object sender, DataObjectEventArgs args)
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
                    //throw (e);
                    Console.WriteLine(e.Message);
                    break;


            }
        }

        #endregion
    }
}
