using System.Net;
using System.Net.Sockets;
using OkoCommon.Game;
using OkoServer;

namespace OkoClient;

public static class Program
{
    // This program is just a placeholder - it will be replaced with UI
    public static void Main()
    {
        var server = new Server();
        
        var serverThread = new Thread(server.AcceptLoop);
        serverThread.Start();
        
        Thread.Sleep(500);
        
        var client = new Client();
        client.PresetName("Alice");
        ConnectToSelf(client);
        
        var client2 = new Client();
        client2.PresetName("Bob");
        ConnectToSelf(client2);

        Thread.Sleep(500);
        
        var player1 = new ClientPlayerLogics(client);
        var player2 = new ClientPlayerLogics(client2);
        
        var playerThread1 = new Thread(player1.PlayerLoop);
        var playerThread2 = new Thread(player2.PlayerLoop);
        
        playerThread1.Start();
        playerThread2.Start();

        var oko = new Game(server.GetPlayers);
        
        while (server.GetPlayers().Count < 2) Thread.Sleep(200);
        
        oko.GameLoop();
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