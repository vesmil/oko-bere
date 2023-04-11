using OkoCommon.Game;
using OkoCommon.Misc;

namespace Tests;

using prob = ProbabilityCalc;

// ReSharper disable once UnusedType.Global
public static class Playground
{
    // ReSharper disable once UnusedMember.Global
    public static void PrintAllProbs()
    {
        var hand = new List<Card> { new(Suit.Hearts, Rank.Seven) };
        Console.Write($"{hand[0]} - ");
        PrintForHand(hand);

        foreach (var rank in Enum.GetValues(typeof(Rank)).Cast<Rank>())
        {
            hand = new List<Card> { new(Suit.Clubs, rank) };
            Console.Write($"{hand[0]} - ");
            PrintForHand(hand);
        }

        Console.WriteLine("");

        hand = new List<Card>
        {
            new(Suit.Clubs, Rank.Seven),
            new(Suit.Spades, Rank.Seven)
        };

        PrintForHand(hand);
    }

    private static void PrintForHand(List<Card> hand)
    {
        var i = 0;
        foreach (var value in prob.NextValues(hand))
        {
            if (value != 0) Console.Write($"{i}: {Math.Round(value * 100, 1)}%, ");

            i++;
        }

        Console.WriteLine();
    }
}