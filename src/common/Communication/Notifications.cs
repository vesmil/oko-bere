using OkoCommon.Game;

namespace OkoCommon.Communication;

public interface INotification<out T>
{
    NotifEnum Type { get; }
    public T? Data { get; }
}

public enum NotifEnum
{
    AskForName,
    GameStart,
    
    NewBanker,
    NewPlayer,
    
    SetInitialBank,
    BankBusted,
    
    AskForTurn,
    
    ReceivedCard,
    CardsDealt,
    
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
    EndOfGame,
}

[Serializable]
public class GenericNotif<T> : INotification<T>
{
    public GenericNotif(NotifEnum type, T data)
    {
        Type = type;
        Data = data;
    }

    public NotifEnum Type { get; init; }
    public T? Data { get; init; }
}

[Serializable]
public class CardNotif : INotification<Card>
{
    public CardNotif(NotifEnum type, Card data)
    {
        Type = type;
        Data = data;
    }

    public NotifEnum Type { get; }
    public Card Data { get; }
}

[Serializable]
public class NoDataNotif : INotification<object>
{
    public NoDataNotif(NotifEnum type)
    {
        Type = type;
    }

    public NotifEnum Type { get; }
    public object? Data => null;
}

[Serializable]
internal class PlayerNotif : INotification<string>
{
    public PlayerNotif(NotifEnum type, PlayerBase player)
    {
        Type = NotifEnum.NewPlayer;
        Data = player.Name;
    }
    
    public NotifEnum Type { get; }
    public string? Data { get; }
}