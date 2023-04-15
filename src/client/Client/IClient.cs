using OkoCommon;
using OkoCommon.Communication;

namespace OkoClient.Client;

public interface IClient
{
    public GameState GameState { get; }
    public Guid PlayerId { get; }

    /// <summary>
    ///     Main loop that receive messages from server
    /// </summary>
    public void PlayerLoop();

    /// <summary>
    ///     If the player is the banker, this method sets the bank.
    /// </summary>
    /// <param name="amount"></param>
    public void BankSet(int amount);

    /// <summary>
    ///     lets the server know that the player is ready to continue
    /// </summary>
    /// <param name="decision"></param>
    public void Continue(bool decision);

    /// <summary>
    ///     Whether the player wants to take a card, bet or end his turn with number value usually representning the bet
    /// </summary>
    /// <param name="decision"></param>
    /// <param name="value"></param>
    public void Turn(TurnDecision decision, int value = 0);

    /// <summary>
    ///     If the player wants to duel, he can specify the bet amount. If the bet is 0, the player declines the duel.
    /// </summary>
    /// <param name="bet"></param>
    public void Duel(int bet);

    /// <summary>
    ///     Selecting where in the deck to cut.
    /// </summary>
    /// <param name="where"></param>
    public void Cut(int where);

    /// <summary>
    ///     The banker can choose a player that will cut the deck.
    /// </summary>
    /// <param name="playerId"></param>
    public void CutPlayer(Guid playerId);

    /// <summary>
    ///     Event that is raised when the client receives a message from the server.
    /// </summary>
    public event EventHandler<MessageReceivedEventArgs>? MessageReceived;
}