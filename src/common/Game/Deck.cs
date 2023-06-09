﻿using System.Collections;

namespace OkoCommon.Game;

/// <summary>
///     Simulates a deck of cards.
/// </summary>
public class Deck
{
    // The cards are stored in a list, so that they can be easily shuffled.
    protected readonly List<Card> Cards = new();
    private readonly Random random = new();

    public Deck()
    {
        foreach (Suit suit in Enum.GetValues(typeof(Suit)))
        foreach (Rank rank in Enum.GetValues(typeof(Rank)))
            Cards.Add(new Card(suit, rank));
    }

    public int Count => Cards.Count;
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

    /// <summary>
    ///     Reset the deck to its unsuffled state.
    /// </summary>
    public void Restart()
    {
        Cards.Clear();

        foreach (Suit suit in Enum.GetValues(typeof(Suit)))
        foreach (Rank rank in Enum.GetValues(typeof(Rank)))
            Cards.Add(new Card(suit, rank));
    }

    /// <summary>
    ///     Draw a card from the deck and remove it from the deck.
    ///     Returns false if the deck is empty.
    /// </summary>
    /// <param name="card">Drawn card</param>
    /// <returns>Whether drawing was successful.</returns>
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

    /// <summary>
    ///     Draw a card from the deck and remove it from the deck.
    ///     Throws an exception if the deck is empty.
    /// </summary>
    /// <returns>Drawn card</returns>
    /// <exception cref="InvalidOperationException">The deck is empty</exception>
    public Card Draw()
    {
        if (Cards.Count == 0) throw new InvalidOperationException("Deck is empty");

        var card = Cards[^1];
        Cards.RemoveAt(Cards.Count - 1);
        return card;
    }

    /// <summary>
    ///     Split the deck into two halfs and change their order.
    /// </summary>
    /// <returns> Visible card on the bottom of the first half. </returns>
    public Card Cut(int cutIndex = -1)
    {
        if (Cards.Count == 0) throw new InvalidOperationException("Deck is empty");

        if (cutIndex == -1) cutIndex = random.Next(0, Cards.Count);

        var firstHalf = Cards.Take(cutIndex).ToList();
        var secondHalf = Cards.Skip(cutIndex).ToList();

        var visibleCard = secondHalf[0];

        Cards.Clear();

        Cards.AddRange(secondHalf);
        Cards.AddRange(firstHalf);

        return visibleCard;
    }
}

/// <summary>
///     Deck used for testing and probability calculations.
/// </summary>
internal class TestDeck : Deck, IEnumerable<Card>
{
    public IEnumerator<Card> GetEnumerator()
    {
        return Cards.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

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
}

// byte is used instead of int to save memory.
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