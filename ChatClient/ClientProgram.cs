using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using ChatProtocol;
using static ChatProtocol.ServerExceptionMessage;
using System.Threading;

namespace ChatClient
{
    class ClientProgram
    {

        static async Task MainAsync(string[] args)
        {
            Client client = new Client(IPAddress.Loopback, 8005);

            client.TextMessageReceived += (_, msg) => 
            {
                var res = (TextMessage)msg;
                Console.WriteLine($"{res.UserName}: {res.Text}");
            }; 
            client.ConnectionNotificationMessageReceived += (_, msg) =>
            {
                var res = (ConnectionNotificationMessage)msg;
                Console.WriteLine(res.UserName + " connected");
            };
            client.DisconnectionNotificationMessageReceived += (_, msg) =>
            {
                var res = (DisconnectionNotificationMessage)msg;
                Console.WriteLine(res.UserName + " disconnected");
            };
            client.ServerStopNotificationMessageReceived += (_, msg) =>
            {
                Console.WriteLine("Server stopped");
                client.DisconnectAsync().Wait();
            };
            client.PersonalMessageReceived += (_, msg) =>
            {
                var res = (PersonalMessage)msg;
                Console.WriteLine($"Personally from {res.SenderName}: {res.Text}");
            };


            string name;
            do
            {
                Console.WriteLine("Enter your username");
                name = Console.ReadLine();
            } while (!await client.ConnectAsync(name));

            Console.WriteLine("connected to server");

            while (true)
            {
                string msg = Console.ReadLine();

                try
                {
                    if (msg == "clm")
                    {
                        Console.WriteLine("Connected clients:");
                        foreach (var un in await client.GetConnectionListAsync())
                        {
                            Console.WriteLine(un);
                        };
                    }
                    else if (msg == "end")
                        break;
                    else if (msg == "pers")
                    {
                        Console.WriteLine("Enter receiver name");
                        string recName = Console.ReadLine();
                        Console.WriteLine("Enter your message");
                        string message = Console.ReadLine();
                        await client.SendPersonallyMessage(recName, message);
                    }

                    else await client.SendTextMessageAsync(msg);

                } catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            await client.DisconnectAsync();
        }

        static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }
    }
}
