using OkoCommon.Communication;

namespace OkoCommon.Game;

public delegate List<PlayerBase> GetPlayersDelegate();

/// <summary>
///     Class defining a player for a card game
/// </summary>
public abstract class PlayerBase
{
    public readonly List<Card> Hand = new();
    public readonly Guid Id;

    public int Balance;
    public int Bet;

    public bool Exchanged;

    public bool IsBanker;
    public string Name;

    protected PlayerBase(string name, int balance)
    {
        if (balance < 0)
            throw new ArgumentOutOfRangeException(nameof(balance), "Balance cannot be negative");

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));

        Name = name;
        Balance = balance;

        Id = Guid.NewGuid();
    }

    public abstract IResponse<T> GetResponse<T>();
    public abstract void Notify<T>(INotification<T> notification);

    public PlayerInfo ToPlayerInfo()
    {
        return new PlayerInfo(this);
    }

    public abstract Task<T?> GetResponseAsync<T>();

    public int Total()
    {
        var possibles = Hand.GetSum();
        return possibles[0];
    }

    public override bool Equals(object? obj)
    {
        if (obj is PlayerBase player)
            return player.Id == Id;

        return false;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public static bool operator ==(PlayerBase left, PlayerBase? right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(PlayerBase left, PlayerBase? right)
    {
        return !(left == right);
    }
}