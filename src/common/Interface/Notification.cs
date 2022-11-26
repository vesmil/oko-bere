﻿namespace OkoCommon.Interface;

public interface INotification<out T>
{
    NotifEnum Type { get; }
    public T? Data { get; }
}

public enum NotifEnum
{
    NewBanker,
    NewPlayer,
    
    SetInitialBank,
    BankBusted,
    
    AskForCardDecision,
    AskBankerForCardDecision,
    
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
}

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

public class NoDataNotif : INotification<object>
{
    public NoDataNotif(NotifEnum type)
    {
        Type = type;
    }

    public NotifEnum Type { get; }
    public object? Data => null;
}