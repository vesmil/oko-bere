namespace OkoCommon.Communication;

public enum TurnDecision : byte
{
    Draw,
    Bet,
    Stop
}

public interface IResponse<out T>
{
    T? Data { get; }
}

public class Response<T> : IResponse<T>
{
    public T? Data { get; init; }
}
