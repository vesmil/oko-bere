using System.Diagnostics;
using OkoCommon.Game;

namespace OkoCommon.Communication;

public enum NotifEnum
{
    AskName,
    UpdateGameState,
    // GameStart,

    NewPlayer,
    PlayerLeft,

    NewBanker,
    AskInitialBank,
    SetInitialBank,
    BankBusted,

    AskTurn,
    AskExchange,

    ReceivedCard,
    Bust,
    Won,
    Lost,

    OtherReceivesCard,
    OtherExchanged,
    OtherBusts,
    OtherWins,

    AskMalaDomu,
    MalaDomuCalled,
    MalaDomuSuccess,

    AskChooseCutPlayer,
    AskChooseCutPosition,
    ShowCutCard,

    AskDuel,
    AskTurnNoBet,

    AskContinue,
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