using System.Net;
using System.Net.Sockets;
using OkoClient;
using OkoCommon.Game;
using OkoServer;

namespace Tests;

public class TcpTests
{
    private List<PlayerBase> players;
    private readonly Server server;

    public TcpTests()
    {
        server = new Server();

        var serverThread = new Thread(server.AcceptLoop);
        var clientInitThread = new Thread(ClientLoop);
        
        serverThread.Start();
        clientInitThread.Start();

        clientInitThread.Join();
        serverThread.Join();

        players = server.GetPlayers();
    }
    
    private static void ClientLoop()
    {
        using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0);
        socket.Connect("8.8.8.8", 65530);
        var endPoint = socket.LocalEndPoint as IPEndPoint;
        var ip = endPoint?.Address.ToString();

        if (ip != null)
        {
            var client = new Client();
            var client2 = new Client();
            
            client.PresetName("Alice");
            client2.PresetName("Bob");
            
            client.Connect(ip, 1234);
            client2.Connect(ip, 1234); ;
            
            return;
        }
        
        throw new Exception("Could not get local IP address");
    }
    
    [Test]
    public void PlayerTest()
    {
        players = server.GetPlayers();  
        
        Assert.That(players, Has.Count.EqualTo(2));
        
        Assert.Multiple(() =>
        {
            Assert.That(players[0].Name, Is.EqualTo("Alice"));
            Assert.That(players[1].Name, Is.EqualTo("Bob"));
        });
    }
    
    [Test]
    public void GameTest()
    {
        // var game = new Game(players);
    }
}