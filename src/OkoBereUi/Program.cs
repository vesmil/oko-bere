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

public class TestTables : Form
{
    private readonly Server server = new();
    private readonly Thread serverThread;
    private readonly Thread gameThread;

    private readonly List<Client> clients = new();
    private readonly List<Thread> clientThreads = new();

    public TestTables()
    {
        serverThread = new Thread(server.AcceptLoop);
        serverThread.Start();

        NewClient("Alice");
        NewClient("Bob");
        NewClient("Cyril");


        foreach (var client in clients)
        {
            var player = new ClientPlayerLogics(client);
            
            var clientThread = new Thread(player.PlayerLoop);
            clientThreads.Add(clientThread);
            clientThread.Start();
        }
        
        while (server.GetPlayers().Count < 2) Thread.Sleep(200);

        gameThread = new Thread(new Game(server.GetPlayers).GameLoop);
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

    private void NewClient(string name)
    {
        var client = new Client();
        clients.Add(client);
        
        client.PresetName(name);
        ConnectToSelf(client);
        
        var ui = new GameTableForm(client);
        ui.Show();
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