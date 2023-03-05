using OkoCommon;
using OkoCommon.Communication;
using OkoCommon.Game;

namespace OkoClient.Client;

// TODO ...
public class SoloTcpClient : IClient
{
    public SoloTcpClient(int numPlayers)
    {
    }

    public void PlayerLoop()
    {
    }

    public void BankSet(int amount)
    {
    }

    public void Continue(bool decision)
    {
    }

    public void Turn(TurnDecision decision)
    {
    }

    public void Turn(TurnDecision decision, int bet)
    {
    }

    public void Duel(bool decision, int bet)
    {
    }

    public void Cut(int where)
    {
    }

    public void CutPlayer(Guid playerId)
    {
    }

    public void CutPlayer(PlayerBase player)
    {
    }

    public event EventHandler<MessageReceivedEventArgs>? MessageReceived;
    public GameState GameState { get; }
}