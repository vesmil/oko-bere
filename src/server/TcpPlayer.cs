using System.Net.Sockets;
using System.Text;
using OkoCommon;
using OkoCommon.Interface;

namespace OkoServer;

public class TcpPlayer : OkoCommon.Interface.PlayerBase
{
    private readonly TcpClient client;
    private readonly NetworkStream stream;

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
        // TODO     
        
        return closest;
    }

    public override IResponse GetResponse()
    {
        // TODO
        throw new NotImplementedException();
    }

    public override bool SendNotification(INotification notification)
    {
        // TODO
        throw new NotImplementedException();
    }
}

/*
public IAction GetAction()
{
    var buffer = new byte[1024];
    var bytesRead = stream.Read(buffer, 0, buffer.Length);
                
    if (bytesRead > 0)
    {
        var message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        Console.WriteLine("Received: " + message);
    }
    
    return new Action();
}
public bool SendAction(IAction action)
{
    var data = Encoding.UTF8.GetBytes("Action");
    stream.Write(data, 0, data.Length);
    
    if (stream.CanRead)
    {
        var buffer = new byte[1024];
        var bytesRead = stream.Read(buffer, 0, buffer.Length);
        var response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        return response == "OK";
    }
    
    return false;
}
*/