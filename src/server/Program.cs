using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using OkoServer.Interface;

namespace OkoServer;

public class Server
{
    private readonly TcpListener server;
    private readonly List<KeyValuePair<TcpClient, NetworkStream>> clients = new();

    private const int Port = 1234;

    public Server()
    {
        Debug.WriteLine("Starting server...");
        
        server = new TcpListener(IPAddress.Any, Port);
        server.Start();
    }
    
    ~Server()
    {
        foreach (var client in clients)
        {
            client.Key.Close();
            client.Value.Close();
        }
        
        server.Stop();
    }
    
    public void AcceptLoop()
    {
        while (true)
        {
            var client = server.AcceptTcpClient();
            
            Debug.WriteLine ("Connection accepted from " + client.Client.RemoteEndPoint);

            var networkStream = client.GetStream();
            
            clients.Add(new KeyValuePair<TcpClient, NetworkStream>(client, networkStream));
            Thread.Sleep(1000);
            
            // Recieve message and form name from client
            var message = new byte[1024];
            var bytesRead = networkStream.Read(message, 0, 1024);
        }
    }

    public void MainLoop()
    {
        while (true)
        {
            foreach (var client in clients)
            {
                if (client.Key.Connected)
                {
                    var buffer = new byte[1024];
                    var bytesRead = client.Value.Read(buffer, 0, buffer.Length);
                    
                    if (bytesRead > 0)
                    {
                        var message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        Console.WriteLine("Received: " + message);
                    }
                    
                    var data = Encoding.UTF8.GetBytes("Hello from server");
                    client.Value.Write(data, 0, data.Length);
                }
            }
            
            Thread.Sleep(5000);
        }
    }
}

public static class Program
{
    public static void Main(string[] args)
    {
        var server = new Server();
        
        var thread = new Thread(server.AcceptLoop);
        thread.Start();
        
        server.MainLoop();
    }
}