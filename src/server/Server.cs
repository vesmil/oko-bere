using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using OkoCommon.Interface;

namespace OkoServer;

public class Server
{
    private readonly TcpListener server;
    private readonly List<TcpPlayer> clients = new(); // <KeyValuePair<TcpClient, NetworkStream>

    private const int Port = 1234;

    public Server()
    {
        Debug.WriteLine("Starting server...");
        
        // net stop hns && net start hns
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
            
            var message = Encoding.UTF8.GetBytes("Enter your name: ");
            networkStream.Write(message, 0, message.Length);

            var buffer = new byte[1024];
            
            // TODO - Do it async
            var bytesRead = networkStream.Read(buffer, 0, 1024);
            var name = Encoding.ASCII.GetString(buffer, 0, bytesRead);
            
            clients.Add(new TcpPlayer(client, networkStream, name, 1000)); // TODO - Get money from elsewhere
            
            // TODO - Not close after two players - but add ready confirmation
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
    
    public List<OkoCommon.Interface.PlayerBase> GetPlayers()
    {
        return new List<OkoCommon.Interface.PlayerBase>(clients);
    }
}