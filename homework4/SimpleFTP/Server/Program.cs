const string ENDMessage = "END";

Console.WriteLine("Enter port number for server: ");
var input = Console.ReadLine();
if (!int.TryParse(input, out int port) || port < 1 || port > 65535)
{
    Console.WriteLine("Invalid port number.");
    return;
}

var server = new Server(port);
_ = server.Start();
Console.WriteLine($"Enter \"{ENDMessage}\" to end the server.");

while (true)
{
    var userInput = Console.ReadLine();
    if (string.Equals(userInput, ENDMessage))
    {
        server.Stop();
        break;
    }
}