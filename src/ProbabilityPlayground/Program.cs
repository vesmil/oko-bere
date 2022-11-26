using OkoCommon;

namespace ProbabilityPlayground;

using prob = ProbabilityCalc;

public static class Program
{
    public static void Main(string[] args)
    {
        var hand = new List<Card>{new (Suit.Hearts, Rank.Seven) };
        Console.Write($"{hand[0]} - ");
        PrintForHand(hand);
        
        foreach (var rank in Enum.GetValues(typeof(Rank)).Cast<Rank>())
        {
            hand = new List<Card>{new (Suit.Clubs, rank) };
            Console.Write($"{hand[0]} - ");
            PrintForHand(hand);
        }

        Console.WriteLine("");

        hand = new List<Card>
        {
            new Card(Suit.Clubs, Rank.Seven),
            new Card(Suit.Spades, Rank.Seven),
        };
        
        PrintForHand(hand);
    }

    private static void PrintForHand(List<Card> hand)
    {
        var i = 0;
        foreach (var value in prob.NextValues(hand))
        {
            if (value != 0)
            {
                Console.Write($"{i}: {Math.Round(value * 100, 1)}%, ");
            }

            i++;
        }
        
        Console.WriteLine();
    }
}