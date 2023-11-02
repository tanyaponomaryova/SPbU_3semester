using ConsoleNetworkChat;
using System.Net;

if (args.Length == 1)
{
    if (int.TryParse(args[0], out int port))
    {
        Server server = new(port);
        await server.Start();
    }
    else Console.WriteLine("Invalid input.");
}
else if (args.Length == 2)
{
    if (int.TryParse(args[0], out int port) && IPAddress.TryParse(args[1], out var _))
    {
        Client client = new(port, args[1]);
        await client.Start();
    }
    else Console.WriteLine("Invalid input.");
}
else Console.WriteLine("Invalid input.");