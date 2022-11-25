namespace OkoCommon;

public class Deck
{
    private readonly List<Card> cards = new();
    private readonly Random random = new();
    
    public Deck()
    {
        foreach (Suit suit in Enum.GetValues(typeof(Suit)))
        foreach (Rank rank in Enum.GetValues(typeof(Rank)))
            cards.Add(new Card(suit, rank));
    }

    public bool IsEmpty => cards.Count == 0;

    public void Shuffle()
    {
        // Each card is swapped with another card at a random index.
        for (var i = 0; i < cards.Count; i++)
        {
            var j = random.Next(i, cards.Count);
            (cards[i], cards[j]) = (cards[j], cards[i]);
        }
    }
    
    public bool TryDraw(out Card card)
    {
        if (cards.Count == 0)
        {
            card = default;
            return false;
        }
        
        card = cards[^1];
        cards.RemoveAt(cards.Count - 1);
        return true;
    }
    
    public Card Draw()
    {
        if (cards.Count == 0)
        {
            throw new InvalidOperationException("Deck is empty");
        }

        var card = cards[^1];
        cards.RemoveAt(cards.Count - 1);
        return card;
    }
    
    /// <summary>
    /// Split the deck into two halfs and change their order.
    /// </summary>
    /// <returns> Visible card on the bottom of the first half. </returns>
    public Card Cut()
    {
        if (cards.Count == 0)
        {
            throw new InvalidOperationException("Deck is empty");
        }
        
        var half = cards.Count / new Random().Next(cards.Count);
        
        var firstHalf = cards.Take(half).ToList();
        var secondHalf = cards.Skip(half).ToList();
        
        var visibleCard = secondHalf[0];
        
        cards.Clear();
        
        cards.AddRange(secondHalf);
        cards.AddRange(firstHalf);
        
        return visibleCard;
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

