﻿using OkoCommon.Communication;

namespace OkoCommon.Game;

[Serializable]
public abstract class PlayerBase
{
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
    }
    
    // Could be replaced with GUID in the future
    public override bool Equals(object? obj)
    {
        if (obj is PlayerBase player)
        {
            return player.Name == Name;
        }
    
        return false;
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }

    public abstract IResponse<T> GetResponse<T>();
    public abstract bool Notify<T> (INotification<T> notification);
    
    public static bool operator ==(PlayerBase left, PlayerBase right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(PlayerBase left, PlayerBase right)
    {
        return !(left == right);
    }
}