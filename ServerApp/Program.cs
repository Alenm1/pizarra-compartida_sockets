using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

class Server
{
    static List<Socket> clients = new List<Socket>();

    static void Main(string[] args)
    {
        Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        serverSocket.Bind(new IPEndPoint(IPAddress.Any, 12345));
        serverSocket.Listen(10);
        Console.WriteLine("Server started. Listening for connections...");

        while (true)
        {
            Socket clientSocket = serverSocket.Accept();
            clients.Add(clientSocket);
            Thread clientThread = new Thread(() => HandleClient(clientSocket));
            clientThread.Start();
        }
    }

    static void HandleClient(Socket clientSocket)
    {
        while (true)
        {
            try
            {
                byte[] buffer = new byte[1024];
                int received = clientSocket.Receive(buffer);
                if (received == 0) break;

                foreach (var client in clients)
                {
                    if (client != clientSocket)
                    {
                        client.Send(buffer, received, SocketFlags.None);
                    }
                }
            }
            catch
            {
                clients.Remove(clientSocket);
                clientSocket.Close();
                break;
            }
        }
    }
}