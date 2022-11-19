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

    ~Client()
    {
        Disconnect();
    }
    
    public void Connect(string ip, int port)
    {
        client.Connect(ip, port);
        stream = client.GetStream();
        
        Console.Write("Enter your name: ");
        var name = Console.ReadLine() ?? string.Empty;
        SendMessage(name);
    }

    private void Disconnect()
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
        var _ = stream?.Read(buffer, 0, buffer.Length);
        var message = Encoding.UTF8.GetString(buffer);
        
        return message;
    }
}