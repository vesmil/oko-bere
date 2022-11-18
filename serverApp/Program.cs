using System.Net;
using System.Net.Sockets;
using System.Text;

namespace OkoBereServer;

public class Server
{
    private readonly TcpListener server;
    private readonly TcpClient client;
    private readonly NetworkStream stream;

    private const int Port = 1234;

    public Server()
    {
        server = new TcpListener(IPAddress.Any, Port);
        
        server.Start();
        client = server.AcceptTcpClient();
        stream = client.GetStream();
        
        var data = new byte[256];
        
        var read = stream.Read(data, 0, data.Length);
        var message = Encoding.UTF8.GetString(data);
        
        Console.WriteLine(message);
        
        stream.Close();
        client.Close();
        server.Stop();
    }
}

public static class Program
{
    public static void Main(string[] args)
    {
        var server = new Server();
        
    }
}