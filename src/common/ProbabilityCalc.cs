namespace OkoCommon;

public static class ProbabilityCalc
{
    public static double GetBustProbability(List<Card> hand)
    {
        var deck = new TestDeck();
        deck.TryRemoveCards(hand);

        var bustCount = deck.Select(card => new List<Card>(hand) { card })
            .Count(newHand => newHand.GetBestValue() > 21 && !newHand.IsInstantWin() && !newHand.IsExchangeable());

        return (double) bustCount / deck.Count;
    }
    
    public static double[] NextValues(List<Card> hand)
    {
        var deck = new TestDeck();
        deck.TryRemoveCards(hand);
        
        var result = new double[32];
        for (var i = 0; i < 22; i++)
        {
            result[i] = 0;
        }
        
        foreach (var card in deck)
        {
            var newHand = new List<Card>(hand) { card };
            var value = newHand.GetBestValue();
            
            result[value] += 1;
        }

        for (var i = 0; i < deck.Count; i++)
        {
            result[i] /= deck.Count;
        }
        
        return result;
    }
    

}