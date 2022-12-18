using OkoCommon.Communication;

namespace OkoCommon.Game;

public delegate List<PlayerBase> PlayersDelegate();

[Serializable]
public abstract class PlayerBase
{
    private readonly Guid id;
    public string Name;
    public int Balance;
    public readonly List<Card> Hand = new();
    
    public bool Exchanged;
    
    protected PlayerBase(string name, int balance)
    {
        if (balance < 0)
            throw new ArgumentOutOfRangeException(nameof(balance), "Balance cannot be negative");
        
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));

        Name = name;
        Balance = balance;
        
        id = Guid.NewGuid();
    }
    
    // Could be replaced with GUID in the future
    public override bool Equals(object? obj)
    {
        if (obj is PlayerBase player)
        {
            return player.id == id;
        }
    
        return false;
    }

    public override int GetHashCode()
    {
        return id.GetHashCode();
    }

    public abstract IResponse<T> GetResponse<T>();
    public abstract bool Notify<T> (INotification<T> notification);
    
    public static bool operator ==(PlayerBase? left, PlayerBase? right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(PlayerBase? left, PlayerBase? right)
    {
        return !(left == right);
    }
}