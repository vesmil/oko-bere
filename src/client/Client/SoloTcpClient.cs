using OkoCommon;

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

    public void ContinueDecision(bool decision)
    {
        throw new NotImplementedException();
    }

    public event EventHandler<MessageReceivedEventArgs>? MessageReceived;
    public GameState GameState { get; }
}