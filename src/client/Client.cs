using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using OkoCommon.Communication;

#pragma warning disable SYSLIB0011

namespace OkoClient;

public class Client
{
    private readonly TcpClient client;
    private NetworkStream? stream;
    private readonly IFormatter formatter = new BinaryFormatter();

    private string name = "";
    
    public Client()
    {
        client = new TcpClient();
    }
    
    public void PresetName(string namePreset)
    {
        name = namePreset;
    }

    public void Connect(string ip, int port)
    {
        client.Connect(ip, port);
        stream = client.GetStream();
        
        if (name == "")
        {
            // TODO
            
            /*
            Console.Write("Enter your name: ");
            var name = Console.ReadLine() ?? string.Empty;
            Console.WriteLine(); 
            */
            
            name = new Random().Next().ToString();
        }
        
        SendResponse(name);
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

    private void SendResponse<T>(T data)
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
            var response = formatter.Deserialize(stream);
            var notification = response as INotification<T>;
            return notification;
        }

        throw new Exception("Stream is null");
    }
}