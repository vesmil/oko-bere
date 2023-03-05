using OkoCommon.Game;

namespace OkoCommon;

public struct GameState
{
    public GameState(List<PlayerBase> players, int bank)
    {
        Players = new List<PlayerInfo>();
        foreach (var player in players) Players.Add(new PlayerInfo(player));
        Hand = new List<Card>();
        Bank = bank;
    }

    public List<PlayerInfo> Players;
    public List<Card> Hand;
    
    public int Bank;
}

public struct PlayerInfo
{
    public PlayerInfo(PlayerBase player)
    {
        Name = player.Name;
        Balance = player.Balance;
        Bet = player.Bet;
        CardCount = player.Hand.Count;
        IsBanker = player.IsBanker;
        Id = player.Id;
    }

    public string Name;
    public int Balance;
    public int Bet;
    public int CardCount;
    public bool IsBanker = false;
    public Guid Id;
}

[Serializable]
public enum TurnState
{
    NotStarted,
    PlayerTurn,

    Duel
    // ...
}