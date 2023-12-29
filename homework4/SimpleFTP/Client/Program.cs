using System.Net;

const string ENDMessage = "END";

// из аргс
Console.WriteLine("Enter port number of server:");
var input = Console.ReadLine();
if (!int.TryParse(input, out int port) || port < 1 || port > 65535)
{
    Console.WriteLine("Invalid port number.");
    return;
}

Console.WriteLine("Enter IP adress of server:");
var ipAddress = Console.ReadLine();
if (!IPAddress.TryParse(input, out _) || ipAddress == null)
{
    Console.WriteLine("Invalid IP address.");
    return;
}

var client = new Client(ipAddress, port);
Console.WriteLine($"Enter \"{ENDMessage}\" to end client.");

// реквест из аргс
while (true)
{
    var request = Console.ReadLine();

    if (string.Equals(request, "END"))
    {
        break;
    }
    else if (request != null)
    {
        var response = await client.ExecuteRequest(request);
        Console.WriteLine(response);
    }
}