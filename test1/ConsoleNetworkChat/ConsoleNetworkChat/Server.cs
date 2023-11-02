using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace ConsoleNetworkChat
{
    public class Server
    {
        int port;
        TcpListener tcpListener;
        CancellationTokenSource cts = new();

        public Server(int port)
        {
            tcpListener = new(IPAddress.Any, port);
            this.port = port;
        }

        public async Task Start()
        {
            tcpListener.Start();
            Console.WriteLine($"Server is launched on port {port}. Waiting for connections...");
            while (!cts.IsCancellationRequested)
            {
                using var tcpClient = await tcpListener.AcceptTcpClientAsync(cts.Token);
                Console.WriteLine("Successful connection to the client.");
                using var reader = new StreamReader(tcpClient.GetStream());
                using var writer = new StreamWriter(tcpClient.GetStream());
                Task.Run(async () => await ReceiveMessageAsync(reader));
                await SendMessageAsync(writer);
            }
        }

        async Task SendMessageAsync(StreamWriter writer)
        {
            Console.WriteLine("To send a message, enter the message and press Enter.");
            while (!cts.IsCancellationRequested)
            {
                string? messageForClient = Console.ReadLine();
                if (String.Equals(messageForClient, "exit"))
                {
                    Stop();
                    break;
                }
                await writer.WriteLineAsync(messageForClient);
                await writer.FlushAsync();
            }
        }

        async Task ReceiveMessageAsync(StreamReader reader)
        {
            while (!cts.IsCancellationRequested)
            {
                string? messageForServer = await reader.ReadLineAsync();
                if (string.IsNullOrEmpty(messageForServer)) continue;
                if (string.Equals(messageForServer, "exit"))
                {
                    Stop();
                }
                Console.WriteLine(">>>" + messageForServer);
            }
        }

        private void Stop()
        {
            cts.Cancel();
        }
    }
}