using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;

namespace ServerApp
{
    class ServerObject
    {
        private List<ClientObject> clientsObjList = new List<ClientObject>();
        private Socket serverSocket;
        private IPAddress ip;
        private int port = 5000;

        public ServerObject(IPAddress ip, int port)
        {
            this.ip = ip;
            this.port = port;
        }

        protected internal void Run()
        {
            Listen();

            while (true)
            {
                Socket clientSocket = AcceptSocket();
                ClientObject clientObject = new ClientObject(clientSocket, this);
                Thread clientThread = new Thread(new ThreadStart(clientObject.Run));
                clientThread.Start();
            }
        }

        protected internal void Listen()
        {
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint endPoint = new IPEndPoint(this.ip, this.port);
            serverSocket.Bind(endPoint);
            serverSocket.Listen(10);
            Console.WriteLine($"Server started on {ip}:{port}");

            this.serverSocket = serverSocket;
        }

        private Socket AcceptSocket()
        {
            Socket clientSocket = this.serverSocket.Accept();
            return clientSocket;
        }

        protected internal void AddClientObj(ClientObject clientObject)
        {
            clientsObjList.Add(clientObject);
        }

        protected internal void DisconnectUser(string id)
        {
            for (int i = 0; i < clientsObjList.Count; i++)
            {
                if (clientsObjList[i].Id == id)
                    clientsObjList.RemoveAt(i);
            }
        }

        protected internal void BroadcastMessage(string id, string message, string username)
        {
            string fullString = $"{username}: {message}";
            byte[] data = Encoding.UTF8.GetBytes(fullString);

            for (int i = 0; i < clientsObjList.Count; i++)
            {
                if (clientsObjList[i].Id != id)
                {
                    clientsObjList[i].clientSocket.Send(data);
                }
            }
        }

        public string GetUsersList()
        {
            StringBuilder userList = new StringBuilder();

            foreach (ClientObject username in clientsObjList)
                userList.Append(username.Username + "\n");

            return userList.ToString();
        }
    }
}
