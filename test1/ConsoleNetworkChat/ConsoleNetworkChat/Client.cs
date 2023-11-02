using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleNetworkChat
{
    public class Client
    {
        int port;
        string ipAddress;
        CancellationTokenSource cts = new();

        public Client(int port, string ipAddress)
        {
            this.port = port;
            this.ipAddress = ipAddress; 
        }

        public async Task Start()
        {
            using var tcpClient = new TcpClient(ipAddress, port);
            using var reader = new StreamReader(tcpClient.GetStream());
            using var writer = new StreamWriter(tcpClient.GetStream());
            Task.Run(() => ReceiveMessageAsync(reader));
            await SendMessageAsync(writer);
        }

        async Task SendMessageAsync(StreamWriter writer)
        {
            Console.WriteLine("To send a message, enter the message and press Enter.");
            while (!cts.IsCancellationRequested)
            {
                string? messageForServer = Console.ReadLine();
                if (string.Equals(messageForServer, "exit"))
                {
                    Stop();
                }
                await writer.WriteLineAsync(messageForServer);
                await writer.FlushAsync(); 
            }
        }

        async Task ReceiveMessageAsync(StreamReader reader)
        {
            while (!cts.IsCancellationRequested)
            {
                string? messageForClient = await reader.ReadLineAsync();
                if (string.IsNullOrEmpty(messageForClient)) continue;
                if (string.Equals(messageForClient, "exit"))
                {
                    Stop();
                }
                Console.WriteLine(">>>" + messageForClient);
            }
        }

        private void Stop()
        {
            cts.Cancel();
        }
    }
}
