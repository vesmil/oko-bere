using System.Diagnostics;
using OkoCommon;
using OkoCommon.Communication;
using OkoCommon.Game;

namespace OkoClient.Client;

// Note this could be probably separated further as most of the logic is agnostic to the communication method.
// But since it won't be used, it didn't make sense to do it now

/// <summary>
///     Client implementation with use of TCP communication and JSON
/// </summary>
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

    // Used to prefill name, usually for testing
    private string NamePreset { get; }

    public GameState GameState { get; private set; } = new();
    public Guid PlayerId { get; init; }

    public event EventHandler<MessageReceivedEventArgs>? MessageReceived;

    /// <summary>
    ///     Main loop for the client
    ///     Periodically checks for updates from the server, handles them and notifies the UI
    /// </summary>
    /// <exception cref="InvalidOperationException">In case of invalid update.</exception>
    public void PlayerLoop()
    {
        while (true)
        {
            var update = transfer.Receive<INotification<object>>();
            Debug.WriteLine(NamePreset + " - " + update.Type);

            try
            {
                HandleNotification(update);
            }
            catch (InvalidOperationException ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
            }

            MessageReceived?.Invoke(this, new MessageReceivedEventArgs(update.Data, update.Type));
            Thread.Sleep(100);
        }
    }

    /// <inheritdoc />
    public void BankSet(int amount)
    {
        transfer.Send(new Response<int> { Data = amount });
    }

    /// <inheritdoc />
    public void Continue(bool decision)
    {
        transfer.Send(new Response<bool> { Data = decision });
    }

    /// <inheritdoc />
    public void Turn(TurnDecision decision, int value)
    {
        transfer.Send(new Response<TurnDecision> { Data = decision });
        if (value != 0) transfer.Send(new Response<int> { Data = value });
    }

    /// <inheritdoc />
    public void Duel(int bet = 0)
    {
        transfer.Send(new Response<int> { Data = bet });
    }

    /// <inheritdoc />
    public void Cut(int where)
    {
        transfer.Send(new Response<int> { Data = where });
    }

    /// <inheritdoc />
    public void CutPlayer(Guid playerId)
    {
        transfer.Send(new Response<Guid> { Data = playerId });
    }
    
    /// <inheritdoc />
    public void Exchange(bool decision)
    {
        transfer.Send(new Response<bool> { Data = decision });
    }
    
    /// <inheritdoc />
    public void MalaDomu(bool decision)
    {
        transfer.Send(new Response<bool> { Data = decision });
    }

    /// <summary>
    ///     Changes regarding the client itself
    /// </summary>
    /// <param name="update"></param>
    /// <exception cref="InvalidOperationException"></exception>
    private void HandleNotification(INotification<object> update)
    {
        switch (update.Type)
        {
            case NotifEnum.NewPlayer:
                AddNewPlayer(update);
                break;

            case NotifEnum.PlayerLeft:
                RemovePlayer(update);
                break;

            case NotifEnum.UpdateGameState:
                UpdateGameState(update);
                break;

            case NotifEnum.NewBanker:
                SetNewBanker(update);
                break;

            case NotifEnum.SetInitialBank:
                SetInitialBank(update);
                break;

            case NotifEnum.ReceivedCard:
                AddReceivedCard(update);
                break;

            case NotifEnum.OtherReceivesCard:
                IncrementOtherPlayerCardCount(update);
                break;
        }
    }

    private void AddNewPlayer(INotification<object> update)
    {
        GameState.Players.Add(GetPlayerInfo(update));
    }

    private void RemovePlayer(INotification<object> update)
    {
        GameState.Players.Remove(GetPlayerInfo(update));
    }

    private void UpdateGameState(INotification<object> update)
    {
        GameState = GetData<GameState>(update);
    }

    private void SetNewBanker(INotification<object> update)
    {
        var bankerInfo = GetPlayerInfo(update);
        var banker = GameState.Players.FirstOrDefault(p => p.Id == bankerInfo.Id);

        if (banker is not null)
            banker.IsBanker = true;
        else
            GameState.Players.Add(bankerInfo);
    }

    private void SetInitialBank(INotification<object> update)
    {
        var value = GetData<int>(update);
        var game = GameState;
        game.Bank = value;
        GameState = game;
    }

    private void AddReceivedCard(INotification<object> update)
    {
        var card = GetData<Card>(update);
        GameState.Hand.Add(card);
        GameState.GetPlayerInfo(PlayerId).CardCount++;
    }

    private void IncrementOtherPlayerCardCount(INotification<object> update)
    {
        GameState.GetPlayerInfo(GetData<Guid>(update)).CardCount++;
    }

    private T GetData<T>(INotification<object> update)
    {
        return (T)(update.Data ??
                   throw new InvalidOperationException($"Data is missing for notification type: {update.Type}"));
    }

    private PlayerInfo GetPlayerInfo(INotification<object> update)
    {
        return GetData<PlayerInfo>(update);
    }
}