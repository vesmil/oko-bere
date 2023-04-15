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

        PlayerId = transfer.Receive<Notification<Guid>>().Data;
        transfer.Send(new Response<string> { Data = name });
    }

    private string NamePreset { get; }
    public GameState GameState { get; private set; } = new();
    public Guid PlayerId { get; init; }

    public event EventHandler<MessageReceivedEventArgs>? MessageReceived;

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

                case NotifEnum.UpdateGameState:
                    GameState = (GameState)(update.Data ?? throw new InvalidOperationException());
                    break;

                case NotifEnum.NewBanker:
                {
                    var bankerInfo = (PlayerInfo)(update.Data ?? throw new InvalidOperationException());
                    var banker = GameState.Players.FirstOrDefault(p => p.Id == bankerInfo.Id);

                    if (banker is not null)
                        banker.IsBanker = true;
                    else
                        GameState.Players.Add(bankerInfo);

                    break;
                }
                case NotifEnum.SetInitialBank:
                {
                    var value = (int)(update.Data ?? throw new InvalidOperationException());

                    var game = GameState;
                    game.Bank = value;
                    GameState = game;

                    break;
                }
                case NotifEnum.ReceivedCard:
                {
                    var card = (Card)(update.Data ?? throw new InvalidOperationException());
                    GameState.Hand.Add(card);
                    GameState.GetPlayerInfo(PlayerId).CardCount++;
                    break;
                }
                case NotifEnum.OtherReceivesCard:
                {
                    GameState.GetPlayerInfo((Guid)(update.Data ?? throw new InvalidOperationException())).CardCount++;
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

    public void Turn(TurnDecision decision, int value)
    {
        transfer.Send(new Response<TurnDecision> { Data = decision });
        if (value != 0) transfer.Send(new Response<int> { Data = value });
    }

    public void Duel(int bet = 0)
    {
        transfer.Send(new Response<int> { Data = bet });
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