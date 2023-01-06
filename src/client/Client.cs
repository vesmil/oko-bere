using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using OkoCommon;
using OkoCommon.Communication;

namespace OkoClient;

/// <summary>
/// Recieved message wrapper for event handling.
/// </summary>
public class MessageReceivedEventArgs : EventArgs
{
    public object? Data { get; }
    public NotifEnum Type { get; }

    public MessageReceivedEventArgs(object? data, NotifEnum type)
    {
        Data = data;
        Type = type;
    }
}

/// <summary>
/// This class is mainly used to send and receive messages to and from the server.
/// </summary>
public class Client
{
    private readonly TcpClient tcpClient;
    private NetworkStream? stream;
    private readonly IFormatter formatter = new BinaryFormatter();
    
    public Client()
    {
        tcpClient = new TcpClient();
    }

    public void Connect(string ip, int port)
    {
        tcpClient.Connect(ip, port);
        stream = tcpClient.GetStream();
    }
    
    ~Client()
    {
        Disconnect();
    }

    private void Disconnect()
    {
        tcpClient.Close();
        stream = null;
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

    public GameState GetGameState()
    {
        var notification = ReceiveNotification<GameState>();
        
        if (notification != null)
        {
            return notification.Data;
        }

        return new GameState();
        // throw new Exception("Notification is null");
    }
}