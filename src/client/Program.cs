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
        
        var player1 = new ClientPlayerLogics(client);
        var player2 = new ClientPlayerLogics(client2);
        
        var playerThread1 = new Thread(player1.PlayerLoop);
        var playerThread2 = new Thread(player2.PlayerLoop);
        
        playerThread1.Start();
        playerThread2.Start();
        
        playerThread1.Join();
        playerThread2.Join();
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