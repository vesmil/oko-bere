using OkoCommon.Game;

namespace Tests;

public class CardTests
{
    [Test]
    public void CorrectCount()
    {
        var orderedDeck = (from Suit suit in Enum.GetValues(typeof(Suit)) 
            from Rank rank in Enum.GetValues(typeof(Rank)) 
            select new Card(suit, rank)).ToList();
        
        Assert.That(orderedDeck, Has.Count.EqualTo(32));
    }

    [Test]
    public void IsValueType()
    {
        var card = new Card(Suit.Clubs, Rank.Ace);
        Assert.That(card, Is.EqualTo(new Card(Suit.Clubs, Rank.Ace)));
    }
    
    [Test]
    public void CorrectValues()
    {
        var card = new Card(Suit.Clubs, Rank.Ace);
       
       Assert.That(card.GetValues(), Has.Count.EqualTo(1));
       Assert.That(card.GetValues()[0], Is.EqualTo(11));
       
       var cards = new List<Card> { card, card };
       Assert.That(cards.IsInstantWin, Is.EqualTo(true));
         
       cards = new List<Card> { card, card, card };
       
       Assert.Multiple(() =>
       {
           Assert.That(cards.IsInstantWin, Is.EqualTo(false));
           Assert.That(cards.GetSum(), Has.Count.EqualTo(1));
           Assert.That(cards.GetSum()[0], Is.EqualTo(33));
       });
        
       cards = new List<Card> { new(Suit.Clubs, Rank.Ace), new(Suit.Hearts, Rank.Seven) };
       Assert.That(cards.IsInstantWin, Is.EqualTo(true));
    }
    
    [Test]
    public void CorrectBestValue()
    {
        var cards = new List<Card> { new(Suit.Clubs, Rank.Nine), new(Suit.Spades, Rank.Seven) };
        Assert.That(cards.GetBestValue(), Is.EqualTo(16));
        
        cards = new List<Card> { new(Suit.Clubs, Rank.Nine), new(Suit.Hearts, Rank.Seven) };
        Assert.That(cards.GetBestValue(), Is.EqualTo(20));
    }
    
    [Test]
    public void CorrectExchange() {
        var cards = new List<Card> { new(Suit.Clubs, Rank.Eight), new(Suit.Hearts, Rank.Seven) };
        Assert.That(cards.IsExchangeable(), Is.EqualTo(true), "Eight and Seven can be exchanged");
        
        cards = new List<Card>{new(Suit.Clubs, Rank.Queen), new(Suit.Spades, Rank.Queen), 
            new(Suit.Hearts, Rank.Queen), new(Suit.Diamonds, Rank.Queen), new(Suit.Clubs, Rank.King)};
        Assert.That(cards.IsExchangeable(), Is.EqualTo(true), "Five pictures can be exchanged");
        
        cards = new List<Card>{new(Suit.Clubs, Rank.Queen), new(Suit.Spades, Rank.Queen), 
            new(Suit.Hearts, Rank.Queen), new(Suit.Diamonds, Rank.Queen), new(Suit.Clubs, Rank.Ace)};
        Assert.That(cards.IsExchangeable(), Is.EqualTo(true), "Fifteen can be exchanged");

        cards = new List<Card> { new(Suit.Clubs, Rank.Nine), new(Suit.Hearts, Rank.Seven) };
        Assert.That(cards.IsExchangeable(), Is.EqualTo(false), "Nine and Seven cannot be exchanged");
    }
    
    [Test]
    public void CorrectToString()
    {
        var card = new Card(Suit.Clubs, Rank.Ace);
        Assert.That(card.ToString(), Is.EqualTo("Ace of Clubs"));
    }
}