using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

using OkoCommon.Communication;
using OkoCommon.Game;

#pragma warning disable SYSLIB0011

namespace OkoServer;

[Serializable]
public class TcpPlayer : PlayerBase
{
    private readonly TcpClient client;
    private readonly NetworkStream stream;
    
    // TODO change to not obsolete thing
    private readonly IFormatter formatter = new BinaryFormatter();

    public TcpPlayer(TcpClient client, NetworkStream stream, string name, int balance) : base(name, balance)
    {
        this.client = client;
        this.stream = stream;
    }

    public void SendString(string message)
    {
        var data = Encoding.ASCII.GetBytes(message);
        stream.Write(data, 0, data.Length);
    }
    
    public string ReceiveString()
    {
        var data = new byte[1024];
        var bytes = stream.Read(data, 0, data.Length);
        return Encoding.ASCII.GetString(data, 0, bytes);
    }

    public void Close()
    {
        stream.Close();
        client.Close();
    }
    
    public bool InstantWin()
    {
        if (Hand.Count == 2)
        {
            if (Hand[0].Rank == Rank.Ace && Hand[1].Rank == Rank.Ace)
                return true;
            
            if ((Hand[0].Rank == Rank.Ace || Hand[1].Rank == Rank.Ace) &&
                (Hand[0].Rank == Rank.Seven && Hand[0].Suit == Suit.Hearts || Hand[1].Rank == Rank.Seven && Hand[0].Suit == Suit.Hearts))
                return true;
        }

        return false;
    }

    public int Total()
    {
        var possibles = Hand.GetSum();

        if (possibles.Count == 1)
        {
            return possibles[0];
        }

        var closest = possibles[0];
        
        return closest;
    }

    public override IResponse<T> GetResponse<T>()
    {
        return (IResponse<T>) formatter.Deserialize(stream);
    }

    public override bool Notify<T>(INotification<T> notification)
    {
        if (stream.CanWrite)
        {
            formatter.Serialize(stream, notification);
            return true;
        }

        return false;
    }
}