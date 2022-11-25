namespace OkoCommon.Interface;

public interface IResponse
{
    // From which plauer...
    //
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

public class PlayerResponse : IResponse
{
    public PlayerResponseEnum Type { get; init; }
    public int? Value { get; init; }
}

public class BankResponse : IResponse
{
    public BankResponseEnum Type { get; init; }
}

public class BooleanResponse : IResponse
{
    public bool Value { get; init; }
}