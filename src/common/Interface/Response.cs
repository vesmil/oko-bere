namespace OkoCommon.Interface;

public interface IResponse<T>
{
    T? Data { get; init; }
}

public enum ResponseType
{
    
}


public enum PlayerResponseEnum : byte
{
    NextCard,
    Bet,
    End,
}

public enum BankResponseEnum : byte
{
    NextCard,
    End,
}

public struct PlayerResponse : IResponse<int>
{
    public int Data { get; init; }
}

public struct BooleanResponse : IResponse<bool>
{
    public bool Data { get; init; }
}