using OkoCommon.Game;

namespace OkoCommon.Communication;

public interface INotification<T>
{
    NotifEnum Type { get; }
    public T? Data { get; }
}

public enum NotifEnum
{
    AskForName,
    GameStateInfo,
    GameStart,

    NewBanker,
    NewPlayer,

    SetInitialBank,
    BankBusted,

    AskForTurn,

    ReceivedCard,
    CardsDealt,
    Bust,

    AskForMalaDomu,
    MalaDomuCalled,
    MalaDomuSuccess,

    ChooseCutPlayer,
    ChooseCutPosition,
    SeeCutCard,

    DuelOffer,
    DuelDeclined,
    DuelAccepted,
    DuelAskNextCard,

    AlreadyExchanged,
    ExchangeAllowed,

    PlayerLeft,

    AskForContinue,
    Continue,
    NotEnoughPlayers,
    EndOfGame
}

public struct GenericNotif<T> : INotification<T>
{
    public GenericNotif(NotifEnum type, T data)
    {
        Type = type;
        Data = data;
    }

    public NotifEnum Type { get; init; }
    public T? Data { get; init; }
}

public struct CardNotif : INotification<Card>
{
    public CardNotif(NotifEnum type, Card data)
    {
        Type = type;
        Data = data;
    }

    public NotifEnum Type { get; }
    public Card Data { get; }
}

public struct NoDataNotif : INotification<object>
{
    public NoDataNotif(NotifEnum type)
    {
        Type = type;
    }

    public NotifEnum Type { get; }
    public object? Data => null;
}

public struct PlayerNotif : INotification<PlayerInfo>
{
    public PlayerNotif(NotifEnum type, PlayerBase player)
    {
        Type = NotifEnum.NewPlayer;
        Data = new PlayerInfo(player.Name, player.Balance, player.Bet, player.Hand.Count);
    }

    public NotifEnum Type { get; }
    public PlayerInfo Data { get; }
}