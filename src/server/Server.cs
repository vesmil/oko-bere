using System.Net;
using System.Net.Sockets;
using OkoCommon.Communication;
using OkoCommon.Game;

namespace OkoServer;

public class Server
{
    private readonly TcpListener server;
    private readonly List<TcpPlayer> clients = new();

    private bool accepting = false;

    private const int Port = 1234;

    public Server()
    {
        Console.WriteLine("Starting server...");
        
        // net stop hns && net start hns
        server = new TcpListener(IPAddress.Any, Port);
        server.Start();
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
                Console.WriteLine($"Server - User with name {newPlayer.Name} connected");
            }
            else continue;

            clients.Add(newPlayer);
        }
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