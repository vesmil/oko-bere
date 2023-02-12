using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using OkoCommon.Communication;
using OkoCommon.Game;

namespace OkoServer;

[Serializable]
public class TcpPlayer : PlayerBase
{
    private readonly TcpClient client;
    private readonly JsonSerializer formatter =  new();
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
                ((Hand[0].Rank == Rank.Seven && Hand[0].Suit == Suit.Hearts) ||
                 (Hand[1].Rank == Rank.Seven && Hand[0].Suit == Suit.Hearts)))
                return true;
        }

        return false;
    }

    public int Total()
    {
        var possibles = Hand.GetSum();

        if (possibles.Count == 1) return possibles[0];

        var closest = possibles[0];

        return closest;
    }

    public override IResponse<T> GetResponse<T>()
    {
        var response = new byte[1024];
        var bytesRead = stream.Read(response, 0, response.Length);
        
        var jsonResponse = System.Text.Encoding.UTF8.GetString(response, 0, bytesRead);
        var objResponse = JsonConvert.DeserializeObject<IResponse<T>>(jsonResponse);

        return objResponse ?? throw new JsonException("Response is null.");
    }

    public override bool Notify<T>(INotification<T> notification)
    {
        if (stream.CanWrite)
        {
            var json = JsonConvert.SerializeObject(notification);
            var data = System.Text.Encoding.UTF8.GetBytes(json);
            stream.Write(data, 0, data.Length);
            
            return true;
        }

        return false;
    }
}