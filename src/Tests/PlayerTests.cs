using OkoCommon.Communication;
using OkoCommon.Game;

namespace Tests;

public class TestPlayer : PlayerBase
{
    public TestPlayer(string name, int balance) : base(name, balance) { }
    public override IResponse<T> GetResponse<T>() => throw new NotImplementedException();
    public override bool Notify<T>(INotification<T> notification) => throw new NotImplementedException();
}

public class PlayerTests
{
    [Test]
    public void TestPlayerCreation()
    {
        var player = new TestPlayer("TestPlayer", 1000);

        Assert.Multiple(() =>
        {
            Assert.That(player.Name, Is.EqualTo("TestPlayer"), "Name is not set correctly");
            Assert.That(player.Balance, Is.EqualTo(1000), "Player balance is not 1000");
        });
    }
    
    [Test]
    public void NegativeBalance()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var _ = new TestPlayer("TestPlayer", -1000);
        }, "Balance cannot be negative");
    }
    
    [Test]
    public void EmptyName()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            var _ = new TestPlayer("", 1000);
        }, "Name cannot be empty");
    }

    [Test]
    public void Equality()
    {
        var player1 = new TestPlayer("TestPlayer", 1000);
        var player2 = new TestPlayer("TestPlayer", 1000);
        
        Assert.Multiple(() =>
        {
            Assert.That(player1, Is.EqualTo(player2), "Players are not equal");
        });
    }
}