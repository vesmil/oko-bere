using OkoCommon;
using OkoCommon.Communication;

namespace OkoClient;

public class ClientLogics
{
    private readonly Client client;
    public GameState GameState { get; } = new();
    
    // private bool isBanker;
    
    private string NamePreset { get; set; } = "";
    
    public event EventHandler<MessageReceivedEventArgs>? MessageReceived;

    public ClientLogics(string ip, int port)
    {
        client = new Client();
        client.Connect(ip, port);
        
        // will ask for name...
    }
    
    public ClientLogics(string name, string ip, int port)
    {
        client = new Client();
        client.Connect(ip, port);
        
        NamePreset = name;
        
        var notif = client.ReceiveNotification<object>();

        if (notif is { Type: NotifEnum.AskForName })
        {
            client.SendGenericResponse(name);
        }
        else
        {
            // TODO problem
        }

        GameState = client.GetGameState();
        
        // TODO add valid balance
        GameState.Players.Add(new PlayerInfo(NamePreset,0,0,0));
    }

    /// <summary>
    /// Main loop to receive messages from server
    /// </summary>
    public void PlayerLoop()
    {
        while (true)
        {
            var update = client.ReceiveNotification<object>();

            if (update?.Type == NotifEnum.AskForName)
            {
                client.SendGenericResponse(NamePreset);
            }
            
            // TODO ...

            // Rest of the updates should go to event handler
            if (update != null)
            {
                MessageReceived?.Invoke(this, new MessageReceivedEventArgs(update.Data, update.Type));
            }
            
            Thread.Sleep(100);
        }
    }
    
    // Bet, draw stop...
    
    // ChooseCutPlayer
    
    // ChooseCutCard 
    
    // AcceptDuel
}