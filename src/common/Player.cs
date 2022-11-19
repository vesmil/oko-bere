namespace OkoCommon;

internal class Player
{
    private Guid id;
    
    public readonly string Name;
    public int Balance;

    public readonly List<Card> Hand = new();

    public Player(string name, int balance)
    {
        id = Guid.NewGuid();
        Name = name;
        Balance = balance;
    }

    public bool Exchnaged = false;

    public bool OptionToChange()
    {
        return Hand.Count switch
        {
            1 when Hand[0].rank == Rank.Seven => true,
            2 when Hand[0].rank == Rank.Eight && Hand[1].rank == Rank.Seven ||
                   Hand[0].rank == Rank.Seven && Hand[1].rank == Rank.Eight => true,
            5 when Hand.All(x => x.IsImage()) => true,
          
            _ => false
        };
    }

    public bool InstantWin()
    {
        if (Hand.Count == 2)
        {
            if (Hand[0].rank == Rank.Ace && Hand[1].rank == Rank.Ace)
                return true;
            
            if ((Hand[0].rank == Rank.Ace || Hand[1].rank == Rank.Ace) &&
                (Hand[0].rank == Rank.Seven && Hand[0].suit == Suit.Hearts || Hand[1].rank == Rank.Seven && Hand[0].suit == Suit.Hearts))
                return true;
        }

        return false;
    }

    public int Total()
    {
        var total = 0;
        var sevenHearts = false;
        
        foreach (var card in Hand)
        {
            if (card.rank != Rank.Seven || card.suit != Suit.Hearts)
                total += card.IsImage() ? 1 : (int)card.rank;
            else sevenHearts = true;
        }

        if (sevenHearts) total += total <= 10 ? 11 : total == 11 ? 10 : 7;

        return total;
    }
}