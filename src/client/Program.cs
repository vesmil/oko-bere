using System.Net;
using System.Net.Sockets;

namespace OkoClient;

public static class Program
{
    public static void Main(string[] args)
    {
        var client = new Client();
        ConnectToSelf(client);
        
        while (true)
        {
            var update = client.ReceiveMessage();
            if (update == "exit") break;
            
            Console.Clear();;
            Console.WriteLine("Current player: " + update);

            if (update.Split()[0] == "You")
            {
                var move = Console.ReadLine() ?? "";
                client.SendMessage(move);
            }
        }
    }

    private static void ConnectToSelf(Client client)
    {
        using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0);
        socket.Connect("8.8.8.8", 65530);
        
        var endPoint = socket.LocalEndPoint as IPEndPoint;
        var ip = endPoint?.Address.ToString();

        if (ip != null) client.Connect(ip, 1234);
    }
}