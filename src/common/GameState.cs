using OkoCommon.Game;

namespace OkoCommon;

public struct GameState
{
    public GameState()
    {
        Players = new List<PlayerInfo>();
        Hand = new List<Card>();
    }

    public List<PlayerInfo> Players;
    public List<Card> Hand;

    public TurnState TurnState = TurnState.NotStarted;
    public object? TurnStateData = null;
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