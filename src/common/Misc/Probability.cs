using OkoCommon.Game;

namespace OkoCommon.Misc;

// Currently unused as single player is not supported
public static class ProbabilityCalc
{
    public static double GetBustProbability(List<Card> hand, List<Card>? leftOut = null)
    {
        var deck = PrepareDeck(hand, leftOut);

        var bustCount = deck.Select(card => new List<Card>(hand) { card })
            .Count(newHand => newHand.GetBestValue() > 21 && !newHand.IsInstantWin());

        return (double)bustCount / deck.Count;
    }

    public static double[] NextValues(List<Card> hand, List<Card>? leftOut = null, bool alreadyExchanged = false)
    {
        var deck = PrepareDeck(hand, leftOut);

        var results = new double[23];
        for (var i = 0; i < 22; i++) results[i] = 0;

        foreach (var card in deck)
        {
            var newHand = new List<Card>(hand) { card };
            var exchangeable = AddValue(newHand, ref results, alreadyExchanged);

            if (exchangeable)
            {
                var exchangeRes = NextValues(new List<Card>(), new List<Card>(leftOut ?? new List<Card>()), true);
                for (var i = 0; i < 22; i++) results[i] += exchangeRes[i];
            }
        }

        for (var i = 0; i < results.Length; i++) results[i] /= deck.Count;

        return results;
    }

    private static TestDeck PrepareDeck(IEnumerable<Card> hand, IReadOnlyCollection<Card>? leftOut)
    {
        var deck = new TestDeck();
        deck.TryRemoveCards(hand);

        if (leftOut != null) deck.TryRemoveCards(leftOut);

        return deck;
    }

    private static bool AddValue(List<Card> hand, ref double[] results, bool alreadyExchanged)
    {
        var value = hand.GetBestValue();

        if (hand.IsInstantWin())
            results[21] += 1;
        else if (value > 21)
            results[22] += 1;
        else if (hand.WouldExchange() && !alreadyExchanged)
            return true;
        else
            results[value] += 1;

        return false;
    }
}