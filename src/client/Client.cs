using System.Diagnostics;
using OkoCommon;
using OkoCommon.Communication;

namespace OkoClient;

/// <summary>
///     Recieved message wrapper for event handling.
/// </summary>
public class MessageReceivedEventArgs : EventArgs
{
    public MessageReceivedEventArgs(object? data, NotifEnum type)
    {
        Data = data;
        Type = type;
    }

    public object? Data { get; }
    public NotifEnum Type { get; }
}

public class Client
{
    private readonly IObjectTransfer transfer;

    public Client(string ip, int port)
    {
        transfer = new JsonTcpTransfer(ip, port);
        
        transfer.Receive<INotification<object>>();
        // TODO Ask for name using UI
        
        transfer.Send("...");
        
        GameState = transfer.Receive<INotification<GameState>>().Data;
    }

    public Client(string name, string ip, int port)
    {
        transfer = new JsonTcpTransfer(ip, port);

        NamePreset = name;

        // Note might be necessary to check response
        transfer.Receive<INotification<object>>();
        transfer.Send(NamePreset);
        
        GameState = transfer.Receive<INotification<GameState>>().Data;
    }

    public GameState GameState { get; }
    private string NamePreset { get; } = "";

    public event EventHandler<MessageReceivedEventArgs>? MessageReceived;

    /// <summary>
    ///     Main loop to receive messages from server
    /// </summary>
    public void PlayerLoop()
    {
        while (true)
        {
            var update = transfer.Receive<INotification<object>>();
            
            Debug.WriteLine(NamePreset + " - " + update?.Type);

            if (update?.Type == NotifEnum.NewPlayer)
            {
                GameState.Players.Add((PlayerInfo) (update.Data ?? throw new InvalidOperationException()));
            }
            
            // TODO add valid balance
            
            // Rest of the updates should go to event handler
            if (update != null) MessageReceived?.Invoke(this, new MessageReceivedEventArgs(update.Data, update.Type));

            Thread.Sleep(100);
        }
    }

    // Bet, draw stop...

    // ChooseCutPlayer

    // ChooseCutCard 

    // AcceptDuel
}