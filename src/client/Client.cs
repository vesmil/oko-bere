using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using OkoCommon.Communication;

namespace OkoClient;

public class Client
{
    private readonly TcpClient client;
    private NetworkStream? stream;
    
    // TODO change to not obsolete thing
    private readonly IFormatter formatter = new BinaryFormatter();
    
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
        
        // Console.Write("Enter your name: ");
        // var name = Console.ReadLine() ?? string.Empty;

        var name = new Random().Next().ToString();
        
        SendMessage(name);
    }
    
    ~Client()
    {
        Disconnect();
    }

    private void Disconnect()
    {
        client.Close();
        stream = null;
    }

    public void SendResponse<T>(T data)
    {
        if (stream != null)
        {
            formatter.Serialize(stream, new GenericResponse<T> { Data = data });
        }
        else
        {
            throw new Exception("Stream is null");
        }
    }
    public INotification<T>? ReceiveNotification<T>()
    {
        if (stream != null)
        {
            var response = formatter.Deserialize(stream) as GenericNotif<T>;
            return response;
        }

        throw new Exception("Stream is null");
    }

    private void SendMessage(string message)
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