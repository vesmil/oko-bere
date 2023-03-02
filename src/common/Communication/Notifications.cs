namespace OkoCommon.Communication;

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

public interface INotification<out T>
{
    public NotifEnum Type { get; }
    public T? Data { get; }
}

public struct Notification<T> : INotification<T>
{
    public Notification(NotifEnum type, T data)
    {
        Type = type;
        Data = data;
    }

    public NotifEnum Type { get; }
    public T? Data { get; }
}

public static class Notification
{
    public static Notification<object> Create(NotifEnum type) => new(type, default!);
    public static Notification<T> Create<T>(NotifEnum type, T data) => new(type, data);
}