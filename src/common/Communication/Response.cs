namespace OkoCommon.Communication;

public interface IResponse<T>
{
    T? Data { get; init; }
}

public enum PlayerResponseEnum : byte
{
    Draw,
    Bet,
    Stop,
}

public enum BankResponseEnum : byte
{
    NextCard,
    End,
}

[Serializable]
public class GenericResponse<T> : IResponse<T>
{
    public T? Data { get; init; }
}

[Serializable]
public struct PlayerResponse : IResponse<int>
{
    public int Data { get; init; }
}

[Serializable]
public struct BooleanResponse : IResponse<bool>
{
    public bool Data { get; init; }
}