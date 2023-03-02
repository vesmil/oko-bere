using System.Diagnostics;
using OkoCommon;
using OkoCommon.Communication;

namespace OkoClient.Client;

public class TcpClient : IClient
{
    private readonly IObjectTransfer transfer;
    
    public TcpClient(string name, string ip, int port)
    {
        transfer = new JsonTcpTransfer(ip, port);

        NamePreset = name;

        // Note might be necessary to check response from server
        transfer.Receive<INotification<object>>();
        transfer.Send(new GenericResponse<string>{Data=name});
        
        GameState = transfer.Receive<INotification<GameState>>().Data;
    }

    public GameState GameState { get; }
    private string NamePreset { get; }

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
            
            if (update?.Type == NotifEnum.PlayerLeft)
            {
                GameState.Players.Remove((PlayerInfo) (update.Data ?? throw new InvalidOperationException()));
            }


            if (update?.Type == NotifEnum.NewBanker)
            {
                var bankerInfo = (PlayerInfo)(update.Data ?? throw new InvalidOperationException());
             
                // find struct from GameState.Players with the same name and modify it
                var player = GameState.Players.First(p => p.Name == bankerInfo.Name);
                
                GameState.Players.Remove(player);
                player.IsBanker = true;
                GameState.Players.Add(player);
            }

            // TODO add valid balance
            
            // Rest of the updates should go to event handler
            if (update != null) MessageReceived?.Invoke(this, new MessageReceivedEventArgs(update.Data, update.Type));

            Thread.Sleep(100);
        }
    }

    public void ContinueDecision(bool decision)
    {
        transfer.Send(new GenericResponse<bool> {Data = decision});
    }
    
    // Bet, draw stop...

    // ChooseCutPlayer

    // ChooseCutCard 

    // AcceptDuel
}