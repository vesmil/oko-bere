using System.Net;
using System.Net.Sockets;
using OkoClient.Forms;
using OkoCommon.Game;
using OkoServer;
using TcpClient = OkoClient.Client.TcpClient;

namespace OkoClient;

/// <summary>
///     Class used for testing purposes.
///     Creates server and three clients.
/// </summary>
public class TestTables : Form
{
    private readonly List<TcpClient> clients = new();
    private readonly List<Thread> clientThreads = new();
    private readonly Thread gameThread;
    private readonly Server server = new();
    private readonly Thread serverThread;

    public TestTables()
    {
        // Creates server
        serverThread = new Thread(server.AcceptLoop);
        serverThread.Start();

        Thread.Sleep(500);

        var ip = GetSelfIp() ?? throw new Exception("Couldn't get own IP address.");

        // Creates three clients with player game loop
        NewClientLogics("Alice", ip);
        NewClientLogics("Bob", ip);
        NewClientLogics("Cyril", ip);

        foreach (var clientThread in clients.Select(client => new Thread(client.PlayerLoop)))
        {
            clientThreads.Add(clientThread);
            clientThread.Start();
        }

        // Waits for three clients to connect
        var players = server.GetClients();
        while (players.Count < 3)
        {
            Thread.Sleep(200);
            players = server.GetClients();
        }

        // Creates game and assigns it to server
        var game = new Game(server.GetClients);
        server.AssignGame(game);

        // Starts game loop
        gameThread = new Thread(game.GameLoop);
        gameThread.Start();
    }

    ~TestTables()
    {
        serverThread.Interrupt();
        gameThread.Interrupt();
        foreach (var clientThread in clientThreads) clientThread.Interrupt();
    }

    private void NewClientLogics(string name, string ip)
    {
        var client = new TcpClient(name, ip, 1234);
        clients.Add(client);

        var ui = new GameTableForm(client);

        ui.Text = name;
        ui.Show();
    }

    private static string? GetSelfIp()
    {
        using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0);
        socket.Connect("8.8.8.8", 65530);
        var endPoint = socket.LocalEndPoint as IPEndPoint;
        var ip = endPoint?.Address.ToString();

        return ip;
    }
}