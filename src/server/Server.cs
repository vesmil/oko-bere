using System.Net;
using System.Net.Sockets;
using OkoCommon.Game;

namespace OkoServer;

public class Server : IDisposable
{
    private readonly List<TcpPlayer> clients = new();
    private readonly TcpListener server;

    private bool accepting;

    public Server(int port = 1234)
    {
        Console.WriteLine("Starting server...");

        server = new TcpListener(IPAddress.Any, port);

        try
        {
            server.Start();
        }
        catch (Exception)
        {
            throw new Exception("Couldn't start server, try - net stop hns && net start hns");
        }
    }

    // NOTE currently relation between server and game is 1:1
    private IGame? Game { set; get; }

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    public async void AcceptLoop()
    {
        accepting = true;

        while (accepting)
        {
            var client = await server.AcceptTcpClientAsync();
            Console.WriteLine("Server - Connection accepted from " + client.Client.RemoteEndPoint);

            var newPlayer = new TcpPlayer(client, 1000);

            if (newPlayer.Name.Trim() == "")
            {
                Console.WriteLine("Server - Player doesn't have a name, disconnecting");
                newPlayer.Dispose();
                continue;
            }

            clients.Add(newPlayer);

            Game?.OnNewPlayer(newPlayer);
        }
    }

    public List<PlayerBase> GetClients()
    {
        return new List<PlayerBase>(clients);
    }

    public void AssignGame(Game currentGame)
    {
        Game = currentGame;
    }

    private void ReleaseUnmanagedResources()
    {
        foreach (var client in clients) client.Dispose();
        server.Stop();
    }

    ~Server()
    {
        ReleaseUnmanagedResources();
    }
}