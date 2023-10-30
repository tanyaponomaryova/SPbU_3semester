using System.Net;
using System.Net.Sockets;

/// <summary>
/// Class for server.
/// </summary>
public class Server
{
    private TcpListener tcpListener;
    private int port;
    private List<Task> clientsList = new();
    private CancellationTokenSource cancellationTokenSource = new();

    /// <summary>
    /// Constructor of server.
    /// </summary>
    /// <param name="port"></param>
    public Server(int port)
    {
        this.port = port;
        tcpListener = new(IPAddress.Any, port);
    }

    /// <summary>
    /// Starts server.
    /// </summary>
    /// <returns></returns>
    public async Task Start()
    {
        tcpListener.Start();

        Console.WriteLine($"The server is running on port number {port}. Waiting for connections...");
        while (!cancellationTokenSource.IsCancellationRequested)
        {
            var client = await tcpListener.AcceptTcpClientAsync(cancellationTokenSource.Token);

            clientsList.Add(Task.Run(async () => await ProcessClientAsync(client)));
        }
    }

    private async Task ProcessClientAsync(TcpClient client)
    {
        using (client)
        {
            var reader = new StreamReader(client.GetStream());
            var writer = new StreamWriter(client.GetStream()) { AutoFlush = true };

            var request = await reader.ReadLineAsync();

            if (request?[0] == '1')
            {
                await ListAsync(request.Substring(2), writer);
            }
            else if (request?[0] == '2')
            {
                await GetAsync(request.Substring(2), writer);
            }
            else
            {
                await writer.WriteAsync("Incorrect request.");
            }
        }
    }

    private async Task ListAsync(string path, StreamWriter writer)
    {
        if (Directory.Exists(path))
        {
            var directories = Directory.GetDirectories(path);
            var files = Directory.GetFiles(path);
            var response = (directories.Length + files.Length).ToString();
            foreach (var file in files)
            {
                response += $" {file} false";
            }
            foreach (var directory in directories)
            {
                response += $" {directory} true";
            }
            await writer.WriteLineAsync(response);
        }
        else
        {
            await writer.WriteLineAsync("-1");
        }
    }

    private async Task GetAsync(string path, StreamWriter writer)
    {
        if (File.Exists(path))
        {
            using var fileReader = new StreamReader(path);
            await writer.WriteLineAsync($"{new FileInfo(path).Length} {fileReader.ReadToEnd()}");
        }
        else
        {
            await writer.WriteLineAsync("-1");
        }
    }

    /// <summary>
    /// Stops server.
    /// </summary>
    public void Stop()
    {
        cancellationTokenSource.Cancel();
        Task.WaitAll(clientsList.ToArray());
        tcpListener.Stop();
    }
}