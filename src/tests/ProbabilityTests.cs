using OkoCommon.Game;
using OkoCommon.Misc;

namespace Tests;

public class ProbabilityTests
{
    [Test]
    public void Basic()
    {
        var hand = new List<Card> { new(Suit.Clubs, Rank.Ace) };
        Assert.That(ProbabilityCalc.GetBustProbability(hand), Is.EqualTo(0));

        hand.Add(new Card(Suit.Clubs, Rank.King));
        Assert.That(ProbabilityCalc.GetBustProbability(hand), Is.InRange(0.233, 0.234));

        hand.Add(new Card(Suit.Clubs, Rank.Seven));
        Assert.That(ProbabilityCalc.GetBustProbability(hand), Is.InRange(0.62, 0.63));

        hand = new List<Card>
        {
            new(Suit.Hearts, Rank.Ace), new(Suit.Clubs, Rank.Jack), new(Suit.Spades, Rank.Jack),
            new(Suit.Diamonds, Rank.Jack), new(Suit.Hearts, Rank.Jack)
        };
        Assert.That(ProbabilityCalc.GetBustProbability(hand), Is.InRange(0.703, 0.704));
    }

    [Test]
    public void SevenOfHearts()
    {
        var hand = new List<Card> { new(Suit.Hearts, Rank.Seven), new(Suit.Clubs, Rank.Jack) };
        Assert.That(ProbabilityCalc.GetBustProbability(hand), Is.EqualTo(0));

        hand = new List<Card> { new(Suit.Hearts, Rank.Seven), new(Suit.Clubs, Rank.Seven) };
        Assert.That(ProbabilityCalc.GetBustProbability(hand), Is.InRange(0.533, 0.534));
    }

    [Test]
    public void SureLoss()
    {
        var hand = new List<Card> { new(Suit.Clubs, Rank.Ace), new(Suit.Spades, Rank.Ace) };
        Assert.That(ProbabilityCalc.GetBustProbability(hand), Is.EqualTo(1));
    }

    [Test]
    public void AllValues()
    {
        var hand = new List<Card> { new(Suit.Clubs, Rank.Ace) };
        var nextValueProbs = ProbabilityCalc.NextValues(hand);

        for (var i = 0; i <= 11; i++) Assert.That(nextValueProbs[i], Is.EqualTo(0));

        for (var i = 12; i < nextValueProbs.Length; i++) Assert.That(nextValueProbs[i], Is.InRange(0, 1));
    }
}