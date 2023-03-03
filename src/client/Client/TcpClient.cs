using System.Diagnostics;
using OkoCommon;
using OkoCommon.Communication;
using OkoCommon.Game;

namespace OkoClient.Client;

public class TcpClient : IClient
{
    private readonly IObjectTransfer transfer;

    public TcpClient(string name, string ip, int port)
    {
        transfer = new JsonTcpTransfer(ip, port);

        NamePreset = name;

        transfer.Receive<INotification<object>>();
        transfer.Send(new Response<string> { Data = name });
    }

    private string NamePreset { get; }
    public GameState GameState { get; private set; } = new();

    public event EventHandler<MessageReceivedEventArgs>? MessageReceived;

    /// <summary>
    ///     Main loop to receive messages from server
    /// </summary>
    public void PlayerLoop()
    {
        while (true)
        {
            var update = transfer.Receive<INotification<object>>();

            Debug.WriteLine(NamePreset + " - " + update.Type);

            switch (update.Type)
            {
                case NotifEnum.NewPlayer:
                    GameState.Players.Add((PlayerInfo)(update.Data ?? throw new InvalidOperationException()));
                    break;
                
                case NotifEnum.PlayerLeft:
                    GameState.Players.Remove((PlayerInfo)(update.Data ?? throw new InvalidOperationException()));
                    break;
                
                case NotifEnum.GameStateInfo:
                    GameState = (GameState)(update.Data ?? throw new InvalidOperationException());
                    break;
                
                case NotifEnum.NewBanker:
                {
                    var bankerInfo = (PlayerInfo)(update.Data ?? throw new InvalidOperationException());

                    // find struct from GameState.Players with the same name and modify it - not the best solution
                    var player = GameState.Players.First(p => p.Name == bankerInfo.Name);

                    GameState.Players.Remove(player);
                    player.IsBanker = true;
                    GameState.Players.Add(player);
                    break;
                }
            }

            MessageReceived?.Invoke(this, new MessageReceivedEventArgs(update.Data, update.Type));

            Thread.Sleep(100);
        }
    }

    public void BankSet(int amount)
    {
        throw new NotImplementedException();
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
        throw new NotImplementedException();
    }

    public void CutPlayer(PlayerBase player)
    {
        throw new NotImplementedException();
    }
}