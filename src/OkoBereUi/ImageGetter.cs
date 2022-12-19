using OkoCommon.Game;

namespace OkoBereUi;

public class ImageGetter
{
    // I could use three arrays instead of dictionaries... but it doesnt matter
    private static readonly Dictionary<Suit, string> SuitNames = new()
    {     {Suit.Clubs, "kule"},
        {Suit.Diamonds, "srdce"},
        {Suit.Hearts, "listy"},
        {Suit.Spades, "zaludy"}
    };
    
    private static readonly Dictionary<Rank, string> RankMap = new()
    {
        { Rank.Seven, "7" },
        { Rank.Eight, "8" },
        { Rank.Nine, "9" },
        { Rank.Ten, "10" },
        { Rank.Jack, "spodek" },
        { Rank.Queen, "dama" },
        { Rank.King, "kral" },
        { Rank.Ace, "eso" }
    };

    private static Dictionary<Card, Image> Images { get; } = new ();

    static ImageGetter()
    {
        foreach (var suite in SuitNames.Keys)
        {
            foreach (var rank in RankMap.Keys)
            {
                var card = new Card(suite, rank);
                
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "res", $"{SuitNames[suite]}_{RankMap[rank]}.png");
                var image = Image.FromFile(imagePath);
                
                Images.Add(card, image);
            }
        }
    }
    
    public static Image GetImage(Card card)
    {
        return Images[card];
    }
}