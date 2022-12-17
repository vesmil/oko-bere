using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using OkoCommon.Communication;
using OkoCommon.Game;

namespace OkoServer;

public class Server
{
    private readonly TcpListener server;
    private readonly List<TcpPlayer> clients = new();

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
    
    public void AcceptLoop()
    {
        while (true)
        {
            var client = server.AcceptTcpClient();
            
            Console.WriteLine ("Connection accepted from " + client.Client.RemoteEndPoint);

            var networkStream = client.GetStream();
            var newPlayer = new TcpPlayer(client, networkStream, "unknown", 1000);

            newPlayer.Notify(new NoDataNotif(NotifEnum.AskForName));
            var nameResponse = newPlayer.GetResponse<string>();

            if (nameResponse.Data != null)
            {
                newPlayer.Name = nameResponse.Data;
                Console.WriteLine($"User with name {newPlayer.Name} connected");
            }
            else continue;

            clients.Add(newPlayer);
            
            if (clients.Count == 2)
            {
                foreach (var player in clients)
                {
                    player.Notify(new NoDataNotif(NotifEnum.GameStart));
                }
                
                return;
            }
        }
    }
    
    public List<PlayerBase> GetPlayers()
    {
        return new List<PlayerBase>(clients);
    }
}