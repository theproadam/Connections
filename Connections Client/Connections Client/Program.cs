using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Connections_Client
{
    class Program
    {
        static bool AllowLoop = true, IndicateDisconnect = true;

        static void Main(string[] args)
        {
            string ip = "127.0.0.1";
            ushort port = 29436;
            VerifyIPPort(out ip, out port);
            Console.Write("Enter a username: ");
            string name = Console.ReadLine();

            Console.Write("Connecting To Messaging Server... ");
            Client server;
            try
            {
                server = Client.Connect(ip, port);
                server.Writer.Write(name + " joined the room.");
            }
            catch
            {
                Console.WriteLine("Failed To Connect To Server!");
                Console.ReadLine();
                return;
            }
            Console.WriteLine("Connected. Type \"/leave\" to disconnect, or \"/status\" for status");
            
            Task.Run(delegate() {
                while (AllowLoop)
                {
                    string readable;
                    try
                    {
                        readable = server.Reader.ReadString(); 
                    }
                    catch {
                        AllowLoop = false;
                        Console.SetCursorPosition(0, Console.CursorTop);
                        if (IndicateDisconnect)
                            Console.WriteLine("Error: Lost Connection To Server");
                        break;
                    }

                    TriggerUpdate(readable);
                }
            });

            while (AllowLoop)
            {
                Console.Write("> ");

                string str = Console.ReadLine();
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                Console.WriteLine("You: " + str);

                try
                {
                    if (str == "/leave")
                    {
                        server.Writer.Write(name + " left the room.");
                        IndicateDisconnect = false;
                        break;
                    }
                    else if (str == "/status")
                    {
                        server.Writer.Write("/status");
                    }
                    server.Writer.Write(name + ": " + str);
                }
                catch
                {
                    Console.WriteLine("Failed To Send Message!");
                }
            }

            server.Close();
            if (AllowLoop) Console.WriteLine("Disconnected From Server");
            Console.ReadLine();
        }

        static void TriggerUpdate(string Text)
        {
            string str = "";

            for (int i = 0; i < Console.CursorLeft; i++)
                str += ConsoleReader.ReadCharacterAt(i, Console.CursorTop);

            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(Text);
          //  Console.Write(str);

            if (Text.Length < str.Length)
                Console.Write(new string(' ', str.Length - Text.Length));

            Console.WriteLine();
            Console.Write(str);
        }

        static void VerifyIPPort(out string IP, out ushort Port)
        {
            IPAddress ip;
            ushort port;
            string str;

            while (true)
            {
                Console.Write("Enter Address and Port: ");
                str = Console.ReadLine();

                if (!str.Contains(":")) Console.WriteLine("Invalid Address!");  
                else if (IPAddress.TryParse(str.Split(':')[0], out ip))
                {
                    if (ushort.TryParse(str.Split(':')[1], out port)) break;
                    else Console.WriteLine("Invalid Port!");   
                } else Console.WriteLine("Invalid IP Address!");       
            }


            IP = str.Split(':')[0];
            Port = port;
        }
    }

}
