using OkoCommon.Communication;

namespace OkoCommon.Game;

public class AiPlayer : PlayerBase
{
    public AiPlayer(string name, int balance) : base(name, balance)
    {
    }

    public override IResponse<T> GetResponse<T>()
    {
        throw new NotImplementedException();
    }

    public override void Notify<T>(INotification<T> notification)
    {
        throw new NotImplementedException();
    }

    public override Task<T?> GetResponseAsync<T>() where T : default
    {
        throw new NotImplementedException();
    }
}