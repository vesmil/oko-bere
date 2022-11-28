using OkoServer;

namespace Tests;

public class PlayerDbTests
{
    [Test]
    public void AddPlayer()
    {
        var db = new PlayerDb();
        db.AddPlayer(new TestPlayer("Bob", 1000));
        
        var player = db.GetPlayer("Bob");
        
        Assert.IsNotNull(player);
    }

    [Test]
    public void RemovePlayer()
    {
        var db = new PlayerDb();
        db.AddPlayer(new TestPlayer("Bob", 1000));
        
        db.RemovePlayer("Bob");
        var player = db.GetPlayer("Bob");
        
        Assert.IsNull(player);
    }

    [Test]
    public void GetPlayerNames()
    {
        var db = new PlayerDb();
        db.AddPlayer(new TestPlayer("Bob", 1000));
        db.AddPlayer(new TestPlayer("Alice", 1000));
        
        var names = db.GetPlayerNames();
        
        Assert.That(names.Count(), Is.EqualTo(2));
    }
    
    [Test]
    public void Empty()
    {
        var db = new PlayerDb();

        var names = db.GetPlayerNames();
        
        Assert.That(names.Count(), Is.EqualTo(0));
    }
    
    [Test]
    public void Unique()
    {
        var db = new PlayerDb();
        
        db.AddPlayer(new TestPlayer("Bob", 1000));
        
        Assert.Throws<ArgumentException>(() => db.AddPlayer(new TestPlayer("Bob", 0)), "Player already exists");
    }
}