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
            client.MessageReceived += msg => Console.WriteLine(msg);

            client.ConnectAsync();

            Console.WriteLine("connected to server");

            while (true)
            {
                string msg = Console.ReadLine();
                if (msg == "clm")
                    await client.SendAsync(new RequestConnectionListMessage(client.Name));
                else if (msg == "end")
                    break;
                else await client.SendAsync(new TextMessage(msg));
            }

            await client.Disconnect();
        }

        static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }
    }
}
