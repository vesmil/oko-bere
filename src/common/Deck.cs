﻿namespace OkoCommon;

internal class Deck
{
    private readonly List<Card> cards = new();

    public Deck()
    {
        var i = 0;
        foreach (Suit suit in Enum.GetValues(typeof(Suit)))
        foreach (Rank rank in Enum.GetValues(typeof(Rank)))
            cards.Add(new Card(suit, rank));
    }

    public void Shuffle()
    {
        var random = new Random();
        for (var i = 0; i < cards.Count; i++)
        {
            var j = random.Next(i, cards.Count);
            (cards[i], cards[j]) = (cards[j], cards[i]);
        }
    }

    public Card Draw()
    {
        var card = cards[^1];
        cards.RemoveAt(cards.Count - 1);
        return card;
    }
}

internal class Card
{
    public readonly Rank rank;
    public readonly Suit suit;

    public Card(Suit suit, Rank rank)
    {
        this.suit = suit;
        this.rank = rank;
    }
    
    public override string ToString()
    {
        return $"{rank} of {suit}";
    }

    public bool IsImage()
    {
        return (int)rank >= 12;
    }
}

internal enum Suit
{
    Clubs,
    Diamonds,
    Hearts,
    Spades
}

internal enum Rank
{
    Seven = 7,
    Eight = 8,
    Nine = 9,
    Ten = 10,
    Jack = 12,
    Queen = 13,
    King = 14,
    Ace = 11
}