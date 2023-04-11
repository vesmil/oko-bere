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

        PlayerId = transfer.Receive<Notification<Guid>>().Data;
        transfer.Send(new Response<string> { Data = name });
    }

    private string NamePreset { get; }
    public GameState GameState { get; private set; } = new();
    public Guid PlayerId { get; init; }

    public event EventHandler<MessageReceivedEventArgs>? MessageReceived;

    /// <summary>
    ///     Main loop to receive messages from server
    /// </summary>
    public void PlayerLoop()
    {
        // NOTE might create some basic abstruct structure for this
        while (true)
        {
            var update = transfer.Receive<INotification<object>>();

            Debug.WriteLine(NamePreset + " - " + update.Type);

            // Note might add try catch here
            switch (update.Type)
            {
                case NotifEnum.NewPlayer:
                    GameState.Players.Add((PlayerInfo)(update.Data ?? throw new InvalidOperationException()));
                    break;
                
                case NotifEnum.PlayerLeft:
                    GameState.Players.Remove((PlayerInfo)(update.Data ?? throw new InvalidOperationException()));
                    break;
                
                case NotifEnum.UpdateGameState:
                    GameState = (GameState)(update.Data ?? throw new InvalidOperationException());
                    break;
                
                case NotifEnum.NewBanker:
                {
                    var bankerInfo = (PlayerInfo)(update.Data ?? throw new InvalidOperationException());

                    GameState.Players.Remove(GameState.Players.First(p => p.Id == bankerInfo.Id));
                    GameState.Players.Add(bankerInfo);

                    break;
                }
                case NotifEnum.SetInitialBank:
                {
                    var value = (int)(update.Data ?? throw new InvalidOperationException());

                    // NOTE should be done in a better way
                    var game = GameState;
                    game.Bank = value;
                    GameState = game;

                    /*
                    var banker = GameState.Players.First(p => p.IsBanker);
                    
                    banker.Balance = value;
                    GameState.Players.Remove(GameState.Players.First(p => p.Id == banker.Id));
                    GameState.Players.Add(banker);
                    */
                    
                    break;
                }
            }

            MessageReceived?.Invoke(this, new MessageReceivedEventArgs(update.Data, update.Type));

            Thread.Sleep(100);
        }
    }

    public void BankSet(int amount)
    {
        transfer.Send(new Response<int> { Data = amount });
    }

    public void Continue(bool decision)
    {
        transfer.Send(new Response<bool> { Data = decision });
    }

    public void Turn(TurnDecision decision)
    {
        transfer.Send(new Response<TurnDecision> { Data = decision });
    }

    public void Turn(TurnDecision decision, int bet)
    {
        transfer.Send(new Response<TurnDecision> { Data = decision });
        transfer.Send(new Response<int> { Data = bet });
    }

    public void Duel(bool decision, int bet = 0)
    {
        transfer.Send(new Response<bool> { Data = decision });

        if (decision)
        {
            transfer.Send(new Response<int> { Data = bet });
        }
    }
    
    public void Cut(int where)
    {
        transfer.Send(new Response<int> { Data = where });
    }
    
    public void CutPlayer(Guid playerId)
    {
        transfer.Send(new Response<Guid> { Data = playerId });
    }
}