using System.Net;
using System.Net.Sockets;
using OkoBereUi;
using OkoClient;
using OkoCommon.Game;
using OkoServer;

namespace OkoBereUi
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new TestTables());
        }
    }
}

/// <summary>
///  Class used for testing purposes.
/// </summary>
public class TestTables : Form
{
    private readonly Server server = new();
    private readonly Thread serverThread;
    private readonly Thread gameThread;

    private readonly List<ClientLogics> clients = new();
    private readonly List<Thread> clientThreads = new();

    public TestTables()
    {
        serverThread = new Thread(server.AcceptLoop);
        serverThread.Start();

        var ip = GetSelfIp();
        
        NewClientLogics("Alice", ip);
        NewClientLogics("Bob", ip);
        NewClientLogics("Cyril", ip);

        foreach (var client in clients)
        {
            var clientThread = new Thread(client.PlayerLoop);
            clientThreads.Add(clientThread);
            clientThread.Start();
        }

        while (server.GetPlayers().Count < 3) 
            Thread.Sleep(200);

        gameThread = new Thread(new Game(server.GetPlayers).GameLoop);
        gameThread.Start();
    }
    
    ~TestTables()
    {
        serverThread.Interrupt();
        gameThread.Interrupt();
        foreach (var clientThread in clientThreads)
        {
            clientThread.Interrupt();
        }
    }

    private void NewClientLogics(string name, string ip)
    {
        var client = new ClientLogics(name, ip, 1234);
        clients.Add(client);
        
        var ui = new GameTableForm(client);
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