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
    
    public override async Task<T?> GetResponseAsync<T>() where T : default
    {
        var responseTask = Task.Run(() => transfer.Receive<IResponse<T>>());
        var completedTask = await Task.WhenAny(responseTask, Task.Delay(TimeSpan.FromSeconds(5)));

        return completedTask == responseTask ? responseTask.Result.Data : default;
    }
    public void Dispose()
    {
        transfer.Dispose();
    }
}