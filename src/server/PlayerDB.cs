using OkoCommon.Game;

namespace OkoServer;

public class PlayerDb
{
    private readonly Dictionary<string, PlayerBase> players = new();

    public PlayerBase? GetPlayer(string name)
    {
        players.TryGetValue(name, out var playerBase);
        return playerBase;
    }

    public void AddPlayer(PlayerBase player)
    {
        if (players.ContainsKey(player.Name))
        {
            throw new ArgumentException("Player already exists");
        }
        
        players.Add(player.Name, player);
    }

    public void RemovePlayer(string name)
    {
        players.Remove(name);
    }

    public IEnumerable<string> GetPlayerNames()
    {
        return players.Keys;
    }
    
    public void Clear()
    {
        players.Clear();
    }
}
