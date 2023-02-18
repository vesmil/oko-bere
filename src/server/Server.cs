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

        foreach (var client in clients) client.Close();
        server.Stop();
    }

    public async void AcceptLoop()
    {
        accepting = true;
        while (accepting)
        {
            var client = await server.AcceptTcpClientAsync();

            Console.WriteLine("Server - Connection accepted from " + client.Client.RemoteEndPoint);

            var networkStream = client.GetStream();
            var newPlayer = new TcpPlayer(client, networkStream, "unknown", 1000);

            newPlayer.Notify(new NoDataNotif(NotifEnum.AskForName));
            var nameResponse = newPlayer.GetResponse<string>();

            if (nameResponse.Data != null)
            {
                newPlayer.Name = nameResponse.Data;
            }
            else
            {
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