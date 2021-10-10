using System;
using System.Net;
using System.Threading;

namespace ServerApp
{
    class Program
    {
        static void Main(string[] args)
        {
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            int port = 5000;

            ServerObject server = new ServerObject(ip, port);
            Thread serverThread = new Thread(new ThreadStart(server.Run));
            serverThread.Start();
        }
    }
}
