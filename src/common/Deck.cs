using System.Collections;

namespace OkoCommon;

public class Deck
{
    protected readonly List<Card> Cards = new();
    public int Count => Cards.Count;
    
    private readonly Random random = new();
    
    public Deck()
    {
        foreach (Suit suit in Enum.GetValues(typeof(Suit)))
        foreach (Rank rank in Enum.GetValues(typeof(Rank)))
            Cards.Add(new Card(suit, rank));
    }

    public bool IsEmpty => Cards.Count == 0;

    public void Shuffle()
    {
        // Each card is swapped with another card at a random index.
        for (var i = 0; i < Cards.Count; i++)
        {
            var j = random.Next(i, Cards.Count);
            (Cards[i], Cards[j]) = (Cards[j], Cards[i]);
        }
    }
    
    public bool TryDraw(out Card card)
    {
        if (Cards.Count == 0)
        {
            card = default;
            return false;
        }
        
        card = Cards[^1];
        Cards.RemoveAt(Cards.Count - 1);
        return true;
    }
    
    public Card Draw()
    {
        if (Cards.Count == 0)
        {
            throw new InvalidOperationException("Deck is empty");
        }

        var card = Cards[^1];
        Cards.RemoveAt(Cards.Count - 1);
        return card;
    }
    
    /// <summary>
    /// Split the deck into two halfs and change their order.
    /// </summary>
    /// <returns> Visible card on the bottom of the first half. </returns>
    public Card Cut()
    {
        if (Cards.Count == 0)
        {
            throw new InvalidOperationException("Deck is empty");
        }
        
        var half = Cards.Count / new Random().Next(Cards.Count);
        
        var firstHalf = Cards.Take(half).ToList();
        var secondHalf = Cards.Skip(half).ToList();
        
        var visibleCard = secondHalf[0];
        
        Cards.Clear();
        
        Cards.AddRange(secondHalf);
        Cards.AddRange(firstHalf);
        
        return visibleCard;
    }
}

internal class TestDeck : Deck, IEnumerable<Card>
{
    public string GetCardsString()
    {
        // Cards are stored in reverse order.
        return string.Join(", ", Cards.Select(c => c.ToString()).Reverse());
    }
    
    public bool TryRemoveCards(IEnumerable<Card> cards)
    {
        return cards.All(card => Cards.Remove(card));
    }
    
    public TestDeck Clone()
    {
        var clone = new TestDeck();
        clone.Cards.AddRange(Cards);
        return clone;
    }


    public IEnumerator<Card> GetEnumerator()
    {
        return Cards.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public enum Suit : byte
{
    Clubs,
    Diamonds,
    Hearts,
    Spades
}

public enum Rank : byte
{
    Seven,
    Eight,
    Nine,
    Ten,
    Jack,
    Queen,
    King,
    Ace
}

