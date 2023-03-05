using System.Diagnostics;
using OkoCommon.Game;

namespace OkoCommon.Communication;

public enum NotifEnum
{
    AskForName,
    GameStateInfo,
    GameStart, //

    NewPlayer,
    PlayerLeft,
    NewBanker,

    SetInitialBank,
    BankBusted,

    AskForTurn,
    AskForExchange,

    ReceivedCard,
    Bust,
    Won,
    
    OtherReceivesCard,
    OtherExchanged,
    OtherBusts,
    OtherWins,

    AskForMalaDomu,
    MalaDomuCalled,
    MalaDomuSuccess,

    ChooseCutPlayer,
    ChooseCutPosition,
    ShowCutCard,

    DuelOffer,
    DuelAccepted,
    DuelDeclined,
    AskForTurnNoBet,
    
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

public readonly struct Notification<T> : INotification<object>
{
    public Notification(NotifEnum type, T data)
    {
        Type = type;
        Data = data;
    }

    public NotifEnum Type { get; }
    public T? Data { get; }
    object? INotification<object>.Data => Data;
}

public static class Notification
{
    public static Notification<object> Create(NotifEnum type)
    {
        return new Notification<object>(type, default!);
    }

    public static Notification<T> Create<T>(NotifEnum type, T data)
    {
        Debug.Assert(typeof(T) != typeof(PlayerBase));
        return new Notification<T>(type, data);
    }
}