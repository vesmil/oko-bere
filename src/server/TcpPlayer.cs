using System.Net.Sockets;
using OkoCommon.Communication;
using OkoCommon.Game;

namespace OkoServer;

public class TcpPlayer : PlayerBase, IDisposable
{
    private readonly IObjectTransfer transfer;
    
    public TcpPlayer(TcpClient client, string? name, int balance) : base("_", balance)
    {
        transfer = new JsonTcpTransfer(client);
        
        if (name != null)
        {
            Name = name;
        }
        else
        {
            Notify(new NoDataNotif(NotifEnum.AskForName));
            var nameResponse = GetResponse<string>();
            
            Name = nameResponse.Data ?? "";
        }
    }
    
    public sealed override IResponse<T> GetResponse<T>()
    {
        return transfer.Receive<IResponse<T>>();
    }

    public sealed override void Notify<T>(INotification<T> notification)
    {
        transfer.Send(notification);
    }

    public override Task<bool> AskForContinueAsync()
    { 
        Notify(new NoDataNotif(NotifEnum.AskForContinue));
        return Task.FromResult(Task.Run(() => transfer.Receive<IResponse<bool>>()).Result.Data);
    }

    public void Dispose()
    {
        transfer.Dispose();
    }
}