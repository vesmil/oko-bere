using OkoCommon.Communication;
using OkoCommon.Game;

namespace Tests;

public class MockPlayer : PlayerBase
{
    public MockPlayer(string name, int balance) : base(name, balance)
    {
    }

    public override IResponse<T> GetResponse<T>()
    {
        return default!;
    }

    public override Task<T?> GetResponseAsync<T>() where T : default
    {
        return Task.FromResult(default(T));
    }

    public override void Notify<T>(INotification<T> notification)
    {
    }
}

public class PlayerTests
{
    [Test]
    public void TestPlayerCreation()
    {
        var player = new MockPlayer("TestPlayer", 1000);

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
            var _ = new MockPlayer("TestPlayer", -1000);
        }, "Balance cannot be negative");
    }

    [Test]
    public void EmptyName()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            var _ = new MockPlayer("", 1000);
        }, "Name cannot be empty");
    }

    [Test]
    public void Equality()
    {
        var player1 = new MockPlayer("TestPlayer", 1000);
        var player2 = new MockPlayer("TestPlayer", 1000);

        Assert.Multiple(() => { Assert.That(player1, Is.Not.EqualTo(player2), "Players are not equal"); });
    }
}