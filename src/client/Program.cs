using System.Net;
using System.Net.Sockets;
using System.Text;

namespace OkoClient;

public class Client
{
    private readonly TcpClient client;
    private NetworkStream? stream;
    private readonly byte[] buffer;

    public Client()
    {
        client = new TcpClient();
        buffer = new byte[1024];
    }

    public void Connect(string ip, int port)
    {
        client.Connect(ip, port);
        stream = client.GetStream();
    }

    public void Disconnect()
    {
        client.Close();
        stream = null;
    }

    public void SendMessage(string message)
    {
        stream?.Write(Encoding.UTF8.GetBytes(message), 0, message.Length);
    }

    public string ReceiveMessage()
    {
        var read = stream?.Read(buffer, 0, buffer.Length);
        var message = Encoding.UTF8.GetString(buffer);
        return message;
    }
}

public static class Program
{
    public static void Main(string[] args)
    {
        // Connection
        var client = new Client();

        string? ip;
        using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
        {
            socket.Connect("8.8.8.8", 65530);
            var endPoint = socket.LocalEndPoint as IPEndPoint;
            ip = endPoint?.Address.ToString();
        }
        
        if (ip == null)
        {
            Console.WriteLine("Could not get local IP");
            return;
        }

        try
        {
            client.Connect(ip, 1234);
        }
        catch (Exception e)
        {
            Console.WriteLine("Could not connect to server");
            return;
        }
        
        while (true)
        {
            client.SendMessage("Hello World");
            
            var message = client.ReceiveMessage();
            Console.WriteLine(message);
        }
    }
}