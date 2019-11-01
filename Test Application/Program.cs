using Cache_Client;
using Overlay;
using System;
using System.Configuration;
using System.Net;
using System.Threading;

namespace Test_Application
{
    class StartMain
    {
        private Client _client;

        public StartMain(IPEndPoint iPEndPoint)
        {

            _client = new Client(iPEndPoint, HandleEvents);
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
            while (IsRunning)
            {

                Console.WriteLine("Press Enter to continue:\n");
                Console.ReadKey();
                Console.WriteLine("\nChoose from the below menu:\n");

                Console.WriteLine("Enter 1 to ADD data");
                Console.WriteLine("Enter 2 to REMOVE data");
                Console.WriteLine("Enter 3 to GET data");
                Console.WriteLine("Enter 4 to CLEAR data");
                Console.WriteLine("Enter 5 to EXIT\n");
                Console.WriteLine("Enter 6 to Start a Thread");
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
                            if (data.Value.Equals("Not Found"))
                                Console.WriteLine("Object Not Found in Cache");
                            else
                                Console.WriteLine(data.ToString());
                            break;
                        case 4:
                            _client.Clear();
                            break;
                        case 5:
                            _client.Dispose();
                            IsRunning = false;
                            break;
                        case 6:
                            //Thread thread = new Thread(KeepAdding);
                            thread.Start(_client);

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
                                if (choice == 0)
                                {
                                    thread.Abort();
                                    break;
                                }
                            }
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
        }

        private void KeepAdding(Object client)
        {
            //Client c = new Client(Convert.ToInt32(ConfigurationSettings.AppSettings["Port"]));
            //c.Initialize();
            Client c = (Client)client;
            //c.Initialize();
            int i = 0;
            while (true)
            {
                c.Add("key" + i, "value" + i);
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

        private object Get()
        {
            string key;
            Console.Write("\n Enter key:");
            key = Console.ReadLine();
            return _client.Get(key);
        }

        void HandleEvents(object sender, CustomEventArgs args)
        {
            Console.WriteLine(args.Message);
        }
    }
}
