using OkoCommon.Game;

namespace OkoCommon;

public class GameState
{
    public int Bank;
    public List<Card> Hand;

    public List<PlayerInfo> Players;

    public GameState()
    {
        Players = new List<PlayerInfo>();
        Hand = new List<Card>();
        Bank = 0;
    }

    public GameState(List<PlayerBase> players, int bank)
    {
        Players = new List<PlayerInfo>();
        foreach (var player in players) Players.Add(new PlayerInfo(player));
        Hand = new List<Card>();
        Bank = bank;
    }

    public PlayerInfo GetPlayerInfo(Guid id)
    {
        var player = Players.FirstOrDefault(p => p.Id == id);
        return player ?? new PlayerInfo();
    }
}

public class PlayerInfo
{
    public int Balance;
    public int Bet;
    public int CardCount;
    public Guid Id;
    public bool IsBanker;

    public string Name;

    public PlayerInfo()
    {
        Name = "InvalidPlayer";
    }

    public PlayerInfo(PlayerBase player)
    {
        Name = player.Name;
        Balance = player.Balance;
        Bet = player.Bet;
        CardCount = player.Hand.Count;
        IsBanker = player.IsBanker;
        Id = player.Id;
    }
}