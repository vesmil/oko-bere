using OkoCommon;

namespace OkoClient.Client;

public interface IClient
{
    GameState GameState { get; }
    public void PlayerLoop();

    // TODO use wrapper?
    public void ContinueDecision(bool decision);
    public event EventHandler<MessageReceivedEventArgs>? MessageReceived;
}