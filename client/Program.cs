using System.Net.Sockets;
using System.Text;

namespace OkoBereClient;

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
        var client = new Client();
        client.Connect("192.168.1.246",1234);
        client.SendMessage("Hello World");       
    }
}