namespace OkoCommon.Communication;

public interface IResponse<T>
{
    T? Data { get; init; }
}

public enum PlayerResponseEnum : byte
{
    Draw,
    Bet,
    Stop
}

public class GenericResponse<T> : IResponse<T>
{
    public T? Data { get; init; }
}

public struct PlayerResponse : IResponse<int>
{
    public int Data { get; init; }
}

public struct BooleanResponse : IResponse<bool>
{
    public bool Data { get; init; }
}