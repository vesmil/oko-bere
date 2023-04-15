namespace OkoCommon.Communication;

/// <summary>
///     Possible decisions for the player to make during their turn.
/// </summary>
public enum TurnDecision : byte
{
    Draw,
    Bet,
    Stop
}

/// <summary>
///     Generic interface for responses to requests from the client.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IResponse<out T>
{
    T? Data { get; }
}

public class Response<T> : IResponse<T>
{
    public T? Data { get; init; }
}