using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace OkoServer;

public class Server
{
    private readonly TcpListener server;
    private readonly List<TcpPlayer> clients = new(); // <KeyValuePair<TcpClient, NetworkStream>

    private const int Port = 1234;

    public Server()
    {
        Debug.WriteLine("Starting server...");
        
        server = new TcpListener(IPAddress.Any, Port);
        server.Start();
    }
    
    ~Server()
    {
        Debug.WriteLine("Stopping server...");
        
        foreach (var client in clients) client.Close();
        server.Stop();
    }
    
    public void AcceptLoop()
    {
        while (true)
        {
            var client = server.AcceptTcpClient();
            
            Debug.WriteLine ("Connection accepted from " + client.Client.RemoteEndPoint);

            var networkStream = client.GetStream();
            
            var message = new byte[1024];
            var bytesRead = networkStream.Read(message, 0, 1024);
            var name = Encoding.ASCII.GetString(message, 0, bytesRead);
            
            clients.Add(new TcpPlayer(client, networkStream, name));
            Thread.Sleep(1000);
            
            if (clients.Count == 2)
            {
                foreach (var player in clients)
                {
                    player.SendString("start");
                }
                
                return;
            }
        }
    }
    
    public List<TcpPlayer> GetPlayers()
    {
        return clients;
    }
}