namespace OkoCommon;

public class Card
{
    public readonly Rank Rank;
    public readonly Suit Suit;

    public Card(Suit suit, Rank rank)
    {
        Suit = suit;
        Rank = rank;
    }
    
    public override string ToString()
    {
        return $"{Rank} of {Suit}";
    }

    public bool IsImage()
    {
        return (int)Rank >= 12;
    }
}