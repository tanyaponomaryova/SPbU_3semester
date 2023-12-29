using System.Net.Sockets;

/// <summary>
/// Class for client.
/// </summary>
public class Client
{
    private int port;
    private string ipAddress;

    /// <summary>
    /// Constructor for client.
    /// </summary>
    /// <param name="ipAddress">IP address of server</param>
    /// <param name="port">port of server</param>
    public Client(string ipAddress, int port)
    {
        this.port = port;
        this.ipAddress = ipAddress;
    }

    /// <summary>
    /// Executes user's request.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<string> ExecuteRequest(string request) // не нужен
    {
        if (request.Length < 2 || request[0] != '1' && request[0] != '2')
        {
            return "Incorrect request format.";
        }
        else if (request[0] == '1')
        {
            return await ListAsync(request[2..]);
        }
        else
        {
            return await GetAsync(request[2..]);
        }
    }

    private async Task<string> ListAsync(string path)
    {
        using var tcpClient = new TcpClient(ipAddress, port);
        using var writer = new StreamWriter(tcpClient.GetStream());
        using var reader = new StreamReader(tcpClient.GetStream());

        await writer.WriteLineAsync($"1 {path}");
        await writer.FlushAsync();
        var response = await reader.ReadToEndAsync();

        if (response[..2] == "-1")
        {
            return "The specified directory was not found.";
        }
        else
        {
            return response; // список или другая высокоур структура данных
        }
    }

    private async Task<string> GetAsync(string path) // возвращает байтовй массив
    {
        using var tcpClient = new TcpClient(ipAddress, port);
        using var writer = new StreamWriter(tcpClient.GetStream());
        using var reader = new StreamReader(tcpClient.GetStream());

        await writer.WriteLineAsync($"2 {path}");
        await writer.FlushAsync();
        var response = await reader.ReadToEndAsync();

        if (response[..2] == "-1")
        {
            return "The specified file was not found."; // чтото сделать - бросить исключение, код ошибки
        }
        else
        {
            return response;
        }
    }
}