using System.Net;
using System.Net.Sockets;
using OkoCommon.Communication;

namespace OkoClient;

public static class Program
{
    public static void Main(string[] args)
    {
        var client = new Client();
        ConnectToSelf(client);
        
        var client2 = new Client();
        ConnectToSelf(client2);
        
        while (true)
        {
            var update = client.ReceiveNotification<object>();
            Console.WriteLine($"Type: {update?.Type.GetType().GetEnumName(update.Type)}");

            if (update?.Data != null)
            {
                Console.WriteLine($"Data: {update.Data}");
            }
            
            // client.SendResponse(); ...
            
            if (update is { Type: NotifEnum.EndOfGame })
            {
                break;
            }
        }
    }

    private static void ConnectToSelf(Client client)
    {
        // Get IP
        using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0);
        socket.Connect("8.8.8.8", 65530);
        var endPoint = socket.LocalEndPoint as IPEndPoint;
        var ip = endPoint?.Address.ToString();

        if (ip != null) client.Connect(ip, 1234);
    }
}