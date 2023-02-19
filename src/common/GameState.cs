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
    public PlayerInfo(string name, int balance, int bet, int cardCount)
    {
        Name = name;
        Balance = balance;
        Bet = bet;
        CardCount = cardCount;
    }

    public string Name;
    public int Balance;
    public int Bet;
    public int CardCount;
    public bool IsBanker = false;
}

[Serializable]
public enum TurnState
{
    NotStarted,
    PlayerTurn,

    Duel
    // ...
}