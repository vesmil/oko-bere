using OkoCommon;
using OkoCommon.Communication;

namespace OkoClient.Client;

public interface IClient
{
    public void PlayerLoop();
    
    public void BankSet(int amount);
    public void Continue(bool decision);

    public void Turn(TurnDecision decision);
    public void Turn(TurnDecision decision, int bet);
    
    public void Duel(bool decision, int bet);
    
    public void Cut(int where);
    public void CutPlayer(Guid playerId);

    public GameState GameState { get; }
    public Guid PlayerId { get; init; }

    public event EventHandler<MessageReceivedEventArgs>? MessageReceived;
}