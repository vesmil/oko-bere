using System.Net;
using System.Net.Sockets;
using OkoCommon;
using OkoCommon.Communication;
using OkoCommon.Game;

namespace OkoServer;

public class Server
{
    private const int Port = 1234;
    private readonly List<TcpPlayer> clients = new();
    private readonly TcpListener server;

    private bool accepting;

    public Server()
    {
        Console.WriteLine("Starting server...");

        server = new TcpListener(IPAddress.Any, Port);

        try
        {
            server.Start();
        }
        catch (Exception)
        {
            Console.WriteLine("Couldn't start server, try - net stop hns && net start hns");
            Environment.Exit(1);
        }
    }

    ~Server()
    {
        Console.WriteLine("Stopping server...");

        // NOTE might make server into disposable...
        foreach (var client in clients) client.Dispose();
        server.Stop();
    }

    public async void AcceptLoop()
    {
        accepting = true;
        while (accepting)
        {
            var client = await server.AcceptTcpClientAsync();
            Console.WriteLine("Server - Connection accepted from " + client.Client.RemoteEndPoint);

            var newPlayer = new TcpPlayer(client, null, 1000);
            
            if (newPlayer.Name.Trim() == "")
            {
                Console.WriteLine("Server - Player doesn't have a name, disconnecting");
                newPlayer.Dispose();
                continue;
            }

            foreach (var oldClient in clients)
            {
                oldClient.Notify(new PlayerNotif(NotifEnum.NewPlayer, newPlayer));
            }

            clients.Add(newPlayer);
            
            var gameState = CreateGameState();
            newPlayer.Notify(new GenericNotif<GameState>(NotifEnum.GameStateInfo, gameState));
        }
    }

    private GameState CreateGameState()
    {
        var gameState = new GameState();

        foreach (var player in clients)
        {
            gameState.Players.Add(new PlayerInfo(player.Name, player.Balance, player.Bet, player.Hand.Count));
        }

        return gameState;
    }

    public void StopAccepting()
    {
        accepting = false;
    }

    public List<PlayerBase> GetPlayers()
    {
        return new List<PlayerBase>(clients);
    }
}