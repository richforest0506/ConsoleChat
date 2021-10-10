using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerApp
{
    class ClientObject
    {
        protected internal Socket clientSocket;
        private ServerObject serverObject;
        private string message;
        public string Id { get; set; }
        public string Username { get; set; }

        public ClientObject(Socket clientSocket, ServerObject serverObject)
        {
            Id = Guid.NewGuid().ToString();
            this.clientSocket = clientSocket;
            this.serverObject = serverObject;
            serverObject.AddClientObj(this);
        }

        public void Run()
        {
            Username = RecvData();
            Console.WriteLine($"User {Username} connected to this chat");

            string connectNotification = $"User {Username} connected to chat!";
            this.serverObject.BroadcastMessage(Id, connectNotification, "Server>> ");

            while (true)
            {
                message = RecvData();

                if (message == "!users")
                {
                    string userList = serverObject.GetUsersList();
                    SendToTarget($"Users online:\n{userList}");
                    continue;
                }

                if (message == null)
                    break;

                Console.WriteLine($"{Username}: {message}");
                this.serverObject.BroadcastMessage(Id, message, Username);
            }
        }

        private string RecvData()
        {
            StringBuilder message = new StringBuilder();

            int dataLength = 0;
            byte[] data = new byte[256];

            try
            {
                do
                {
                    dataLength = clientSocket.Receive(data);
                    message.Append(Encoding.UTF8.GetString(data, 0, dataLength));

                } while (clientSocket.Available > 0);

                return message.ToString();
            }
            catch
            {
                Console.WriteLine($"User {Username} is disconnected!");
                this.serverObject.BroadcastMessage(Id, $"User {Username} is disconnected!", "Server>> ");
                serverObject.DisconnectUser(Id);
                return null;
            }
           
        }

        private void SendToTarget(string message)
        {
            byte[] data = Encoding.ASCII.GetBytes(message);
            clientSocket.Send(data);
        }
    }
}
