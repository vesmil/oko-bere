using OkoCommon;
using OkoCommon.Game;

namespace Tests;

public class DeckTests
{
    [Test]
    public void Emptiness()
    {
        var deck = new Deck();
        Assert.That(deck.IsEmpty, Is.EqualTo(false));

        for (var i = 0; i < 32; i++)
        {
            deck.Draw();
        }

        Assert.That(deck.IsEmpty, Is.EqualTo(true));
        Assert.That(deck.TryDraw(out _), Is.EqualTo(false));
        Assert.Throws<InvalidOperationException>(() => deck.Draw());
    }
    
    [Test]
    public void Uniqueness()
    {
        var deck = new Deck();
        var cards = new List<Card>();
        
        for (var i = 0; i < 32; i++)
        {
            cards.Add(deck.Draw());
        }

        Assert.That(cards.Distinct().Count(), Is.EqualTo(32));
    }
}