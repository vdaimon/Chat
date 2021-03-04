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
            Console.WriteLine("Enter your username");
            Client client = new Client(IPAddress.Loopback, 8005, Console.ReadLine());

            client.TextMessageReceived += (_, msg) => Console.WriteLine(msg);
            client.ConnectionNotificationMessageReceived += (_, msg) => Console.WriteLine(msg);
            client.DisconnectionNotificationMessageReceived += (_, msg) => Console.WriteLine(msg);
            client.ConnectionListMessageReceived += (_, msg) => Console.WriteLine(msg);
            client.ServerStopNotificationMessageReceived += (_, msg) => Console.WriteLine(msg);

            await client.ConnectAsync();
            client.Listen();

            Console.WriteLine("connected to server");

            while (true)
            {
                string msg = Console.ReadLine();
                if (msg == "clm")
                    await client.SendAsync(new RequestConnectionListMessage(client.Name));
                else if (msg == "end")
                    break;
                else await client.SendAsync(new TextMessage(msg, client.Name));
            }

            await client.Disconnect();
        }

        static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }
    }
}
