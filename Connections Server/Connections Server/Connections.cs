using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Connections_Server
{
    public class Server
    {
        Thread serverThread;
        TcpListener tListener;
        public delegate void IncomingData(Client d);
        public delegate void ClientableList(List<Client> connectedClients);

        IncomingData dataIncoming;

        List<Client> connectedClients;
        object lockable = new object();

        public int ClientCount { get { lock (lockable) return connectedClients.Count;} }

        public Server(IncomingData action, int Port)
        {
            connectedClients = new List<Client>();
            tListener = new TcpListener(IPAddress.Any, Port);
            serverThread = new Thread(new ThreadStart(DataAwait));
            serverThread.IsBackground = true;
            dataIncoming = action;
        }

        public void Start()
        {
            tListener.Start();
            serverThread.Start();
        }

        public void Stop()
        {
            tListener.Stop();
            serverThread.Abort();
        }

        public void GetClients(ClientableList test)
        {
            lock (lockable)
                test(connectedClients);
        }

        void DataAwait()
        {
            while (true)
            {
                if (tListener.Pending())
                {
                    Task.Run(delegate()
                    {
                        TcpClient client = tListener.AcceptTcpClient();
                        Client cC = new Client(client);
                        lock (lockable)
                            connectedClients.Add(cC);

                        dataIncoming(cC);

                        lock (lockable)
                            connectedClients.Remove(cC);

                        client.Close();
                    });
                    
                }
                Thread.Sleep(250);
            }

        }
    }

    public class Client
    {
        TcpClient sourceClient;
        public BinaryWriter Writer;
        public BinaryReader Reader;

        public EndPoint GetIPAddress()
        {
            return sourceClient.Client.RemoteEndPoint;
        }

        public Client(TcpClient tClient)
        {
            sourceClient = tClient;
            Writer = new BinaryWriter(tClient.GetStream());
            Reader = new BinaryReader(tClient.GetStream());
        }

        public static Client Connect(string IP, ushort Port)
        {
            TcpClient tClient = new TcpClient(IP, Port);
            return new Client(tClient);
        }

        public void Close()
        {
            Writer.Close();
            Reader.Close();
        }
    }
}
