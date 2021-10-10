using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ClientApp
{
    class Program
    {
        static Socket serverSocket;
        static IPAddress ip;
        static int port;
        static string username;

        static void Main(string[] args)
        {
            Console.Write("Enter your name: ");
            username = Console.ReadLine();

            if (username == "")
                Environment.Exit(0);

            serverSocket = ConnetToServer();

            Thread threadRecv = new Thread(new ThreadStart(RecvData));
            threadRecv.Start();

            Login();
            SendData();
        }


        static Socket ConnetToServer()
        {
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ip = IPAddress.Parse("127.0.0.1");
            port = 5000;
            IPEndPoint endPoint = new IPEndPoint(ip, port);
            serverSocket.Connect(endPoint);
            return serverSocket;
        }


        static void Login()
        {
            byte[] data = new byte[username.Length];
            data = Encoding.UTF8.GetBytes(username);

            serverSocket.Send(data);
        }

        static void SendData()
        {
            while (true)
            {
                string message = Console.ReadLine();

                byte[] data = new byte[message.Length];
                data = Encoding.UTF8.GetBytes(message);

                serverSocket.Send(data);
            }
        }


        static void RecvData()
        {
            while (true)
            {
                byte[] data = new byte[256];
                int dataLength = 0;

                StringBuilder stringBuider = new StringBuilder();

                try
                {
                    do
                    {
                        dataLength = serverSocket.Receive(data);
                        stringBuider.Append(Encoding.UTF8.GetString(data, 0, dataLength));

                    } while (serverSocket.Available > 0);

                    Console.WriteLine(stringBuider.ToString());

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }



    }
}
