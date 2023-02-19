using OkoCommon.Communication;

namespace OkoCommon.Game;

public delegate List<PlayerBase> GetPlayersDelegate();

[Serializable]
public abstract class PlayerBase
{
    public readonly List<Card> Hand = new();
    private readonly Guid id;
    
    public int Balance;
    public int Bet;

    public bool Exchanged;
    public string Name;

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
    
    public abstract IResponse<T> GetResponse<T>();
    public abstract void Notify<T>(INotification<T> notification);

    public bool InstantWin()
    {
        if (Hand.Count == 2)
        {
            if (Hand[0].Rank == Rank.Ace && Hand[1].Rank == Rank.Ace)
                return true;

            if ((Hand[0].Rank == Rank.Ace || Hand[1].Rank == Rank.Ace) &&
                ((Hand[0].Rank == Rank.Seven && Hand[0].Suit == Suit.Hearts) ||
                 (Hand[1].Rank == Rank.Seven && Hand[0].Suit == Suit.Hearts)))
                return true;
        }

        return false;
    }

    public int Total()
    {
        var possibles = Hand.GetSum();
        return possibles[0];
    }
    
    public override bool Equals(object? obj)
    {
        if (obj is PlayerBase player) 
            return player.id == id;

        return false;
    }

    public override int GetHashCode()
    {
        return id.GetHashCode();
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