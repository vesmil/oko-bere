namespace OkoCommon.Game;

[Serializable]
public readonly struct Card
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

    private static readonly Dictionary<Rank, int> ValueDictionary = new()
    {
        { Rank.Seven, 7 },
        { Rank.Eight, 8 },
        { Rank.Nine, 9 },
        { Rank.Ten, 10 },
        { Rank.Jack, 1 },
        { Rank.Queen, 1 },
        { Rank.King, 1 },
        { Rank.Ace, 11 }
    };

    public List<int> GetValues()
    {
        if (Rank == Rank.Seven && Suit == Suit.Hearts) return new List<int> { 7, 10, 11 };

        return new List<int> { ValueDictionary[Rank] };
    }

    public int GetStandardValue()
    {
        return ValueDictionary[Rank];
    }
}

public static class CardExtensions
{
    public static List<int> GetSum(this IEnumerable<Card> cards)
    {
        List<int> results = new() { 0 };

        foreach (var values in cards.Select(card => card.GetValues()))
            results = (from result in results from value in values select result + value).ToList();

        return results;
    }

    public static int GetBestValue(this IEnumerable<Card> cards)
    {
        var possibleSums = cards.GetSum();

        return possibleSums.Any(sum => sum <= 21) ? possibleSums.Where(sum => sum <= 21).Max() : possibleSums[0];
    }

    public static bool IsBust(this IEnumerable<Card> cards)
    {
        return cards.GetSum().All(value => value > 21);
    }

    public static bool IsInstantWin(this List<Card> cards)
    {
        return cards.Count == 2 &&
               (
                   (cards[0].Rank == Rank.Ace && cards[1].Rank == Rank.Ace) ||
                   (cards[0].Rank == Rank.Seven && cards[0].Suit == Suit.Hearts && cards[1].Rank == Rank.Ace) ||
                   (cards[1].Rank == Rank.Seven && cards[1].Suit == Suit.Hearts && cards[0].Rank == Rank.Ace)
               );
    }

    public static bool WouldExchange(this List<Card> cards)
    {
        if (cards.Count == 1 && cards[0].Rank == Rank.Seven && cards[0].Suit == Suit.Hearts) return false;

        return IsExchangeable(cards);
    }

    public static bool IsExchangeable(this List<Card> cards)
    {
        return cards.Count switch
        {
            1 when cards[0].Rank == Rank.Seven => true,
            2 when (cards[0].Rank == Rank.Eight && cards[1].Rank == Rank.Seven) ||
                   (cards[0].Rank == Rank.Seven && cards[1].Rank == Rank.Eight) => true,
            5 when cards.All(x => x.GetStandardValue() == 1) => true,

            _ => cards.GetSum().Contains(15)
        };
    }
}