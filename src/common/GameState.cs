using OkoCommon.Game;

namespace OkoCommon;

[Serializable]
public struct GameState
{
    public GameState()
    {
    }

    public readonly List<PlayerInfo> Players = new();
    public readonly List<Card> Hand = new();

    public TurnState TurnState = TurnState.NotStarted;
    public object? TurnStateData = null;
}

[Serializable]
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