using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Connections_Server
{
    class Program
    {
        static Server server;
        static int MaxUsers = 4;

        static void Main(string[] args)
        {
            Console.WriteLine("Starting Local Server...");
            server = new Server(ProcessRequest, 29436);
            server.Start();

            Console.ReadLine();
        }

        static void ProcessRequest(Client client)
        {
            Console.WriteLine(client.GetIPAddress().ToString() + " connected");
            bool ClientAuthorized = false;

            if (server.ClientCount > MaxUsers)
            {
                client.Writer.Write("Sorry, The Server is full!");
            }
            else ClientAuthorized = true;

            while (ClientAuthorized)
            {
                try
                {
                    string str = client.Reader.ReadString();

                    if (str == "/status")
                    {
                        Console.WriteLine(client.GetIPAddress().ToString() + " requested status");
                        int userCount = server.ClientCount;
                        client.Writer.Write("There are " + (userCount - 1) + " other users here.\nThe server has " 
                            + userCount + " out of " + MaxUsers + " slots populated.");
                       // client.Writer.Write(server.ClientCount + "/" + MaxUsers + "connected.");
                    }
                    else
                    {
                        Console.WriteLine(str);
                        server.GetClients(delegate(List<Client> clients)
                        {
                            for (int i = 0; i < clients.Count; i++)
                                if (!object.ReferenceEquals(clients[i], client))
                                    clients[i].Writer.Write(str);
                        });
                    }  
                }
                catch
                {
                    break;
                }

            }

            Console.WriteLine(client.GetIPAddress().ToString() + " disconnected");
        }
    }

}
