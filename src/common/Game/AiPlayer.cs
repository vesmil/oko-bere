using OkoCommon.Communication;

namespace OkoCommon.Game;

public class AiPlayer : PlayerBase
{
    public AiPlayer(string name, int balance) : base(name, balance)
    {
    }

    public override IResponse<T> GetResponse<T>()
    {
        return default!;
    }

    public override void Notify<T>(INotification<T> notification)
    {
    }

    public override Task<T?> GetResponseAsync<T>() where T : default
    {
        return default!;
    }
}