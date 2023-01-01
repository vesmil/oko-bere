using OkoCommon.Game;

namespace OkoClient;

public struct GameState
{
    public List<PlayerInfo> Players;
    public List<Card> Hand;
    
    public TurnState TurnState;
    public object TurnStateData;
}

public struct PlayerInfo
{
    // guid, name, count of cards, balance, isBanker, ...
    public string Name;
    public int Balance;
    public bool IsBanker;
    public int Bet;
    public int CardCount;
}

public enum TurnState
{
    NotStarted,
    PlayerTurn,
    Duel,
    // ...
}