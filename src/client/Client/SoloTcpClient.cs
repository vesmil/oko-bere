using OkoCommon;
using OkoCommon.Communication;
using OkoCommon.Game;

namespace OkoClient.Client;

public class SoloTcpClient : IClient
{
    public SoloTcpClient(int numPlayers)
    {
    }

    public void PlayerLoop()
    {
        throw new NotImplementedException();
    }

    public void BankSet(int amount)
    {
        throw new NotImplementedException();
    }

    public void Continue(bool decision)
    {
        throw new NotImplementedException();
    }

    public void Turn(TurnDecision decision)
    {
        throw new NotImplementedException();
    }

    public void Turn(TurnDecision decision, int bet)
    {
        throw new NotImplementedException();
    }

    public void Duel(bool decision, int bet)
    {
        throw new NotImplementedException();
    }

    public void Cut(int where)
    {
        throw new NotImplementedException();
    }

    public void CutPlayer(PlayerBase player)
    {
        throw new NotImplementedException();
    }

    public event EventHandler<MessageReceivedEventArgs>? MessageReceived;
    public GameState GameState { get; }
}