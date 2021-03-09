using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using ChatProtocol;

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
            client.ConnectionListMessageReceived += (_, msg) => 
            {
                var res = (ConnectionListMessage)msg;
                Console.WriteLine("Connection clients");
                foreach (var el in res.UserNames)
                    Console.WriteLine(el);
            };
            client.ServerStopNotificationMessageReceived += (_, msg) =>
            {
                Console.WriteLine("Server stopped");
                client.DisconnectAsync().Wait();
            };
            client.SuccessfulAuthorizationNotificationMessageReceived += (_, msg) =>
            {
                Console.WriteLine(msg);
            };
            client.ClientToClientMessageReceived += (_, msg) =>
            {
                var res = (ClientToClientTextMessage)msg;
                Console.WriteLine($"Personally from {res.SenderName}: {res.Text}");
            };

            bool connected = false;
            do
            {
                Console.WriteLine("Enter your username");
                string name = Console.ReadLine();
                connected = await client.ConnectAsync(name);
                if (!connected)
                    Console.WriteLine("The name is already in use");
            } while (!connected);

            Console.WriteLine("connected to server");

            while (true)
            {
                string msg = Console.ReadLine();
                if (msg == "clm")
                    await client.SendRequestConnectionListAsync();
                else if (msg == "end")
                    break;
                else if (msg == "pers")
                {
                    Console.WriteLine("Enter receiver name");
                    string name = Console.ReadLine();
                    Console.WriteLine("Enter your message");
                    string message = Console.ReadLine();
                    await client.SendPersonallyMessage(name, message);
                }
                    
                else await client.SendTextMessageAsync(msg);
            }

            await client.DisconnectAsync();
        }

        static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }
    }
}
