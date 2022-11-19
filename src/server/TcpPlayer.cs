using System.Net.Sockets;
using System.Text;
using OkoCommon;

namespace OkoServer;

public class TcpPlayer
{
    private readonly TcpClient client;
    private readonly NetworkStream stream;
    
    public readonly string Name;
    public int Balance = 1000;

    public readonly List<Card> Hand = new();
    public bool Exchnaged = false;

    public TcpPlayer(TcpClient client, NetworkStream stream, string name)
    {
        this.client = client;
        this.stream = stream;
        this.Name = name; 
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

    public bool OptionToChange()
    {
        return Hand.Count switch
        {
            1 when Hand[0].Rank == Rank.Seven => true,
            2 when Hand[0].Rank == Rank.Eight && Hand[1].Rank == Rank.Seven ||
                   Hand[0].Rank == Rank.Seven && Hand[1].Rank == Rank.Eight => true,
            5 when Hand.All(x => x.IsImage()) => true,
          
            _ => false
        };
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
        var total = 0;
        var sevenHearts = false;
        
        foreach (var card in Hand)
        {
            if (card.Rank != Rank.Seven || card.Suit != Suit.Hearts)
                total += card.IsImage() ? 1 : (int)card.Rank;
            else sevenHearts = true;
        }

        if (sevenHearts) total += total <= 10 ? 11 : total == 11 ? 10 : 7;

        return total;
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