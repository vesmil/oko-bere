using System.Diagnostics;
using OkoCommon.Game;

namespace OkoCommon.Communication;

/// <summary>
///     Possible updates from the server.
/// </summary>
public enum NotifEnum
{
    AskName,
    UpdateGameState,

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
    OtherBusts,
    OtherWins,
    OtherLost,
    OtherDuel,

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

/// <summary>
///     Generic interface for notifications from the server.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface INotification<out T>
{
    public NotifEnum Type { get; }
    public T? Data { get; }
}

/// <summary>
///     Generic notification from the server.
/// </summary>
/// <typeparam name="T"></typeparam>
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

/// <summary>
///     Factory for creating notifications.
/// </summary>
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