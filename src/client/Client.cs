using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using OkoCommon.Communication;

namespace OkoClient;

public class MessageReceivedEventArgs : EventArgs
{
    public MessageReceivedEventArgs()
    {
        
    }

    // INotification<object> notification { get; set; }
}

public class Client
{
    private readonly TcpClient client;
    private NetworkStream? stream;
    private readonly IFormatter formatter = new BinaryFormatter();

    public event EventHandler<MessageReceivedEventArgs>? MessageReceived;
    
    public string Name = "";
    
    public Client()
    {
        client = new TcpClient();
    }
    
    public void OnMessageReceived()
    {
        
        
        MessageReceived?.Invoke(this, new MessageReceivedEventArgs { });
    }
    
    public void PresetName(string namePreset)
    {
        Name = namePreset;
    }

    public void Connect(string ip, int port)
    {
        client.Connect(ip, port);
        stream = client.GetStream();
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

    public GameState GetGameState()
    {
        // TODO ...
        return new GameState();
    }
    
    public void SendGenericResponse<T>(T data)
    {
        Send(new GenericResponse<T> { Data = data });
    }
    
    public void SendResponse<T>(IResponse<T> response)
    {
        Send(response);
    }

    private void Send(object data)
    {
        if (stream != null)
        {
            formatter.Serialize(stream, data);
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