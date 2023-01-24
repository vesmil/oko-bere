using OkoCommon.Game;

namespace OkoClient;

public static class UiCommon
{
    private static readonly bool IsInitialized;

    private static readonly Font MenuFont = new("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point, 204);
    private static readonly Color UiColor = Color.FromArgb(161, 87, 72);
    private static readonly Color UiOverColor = Color.FromArgb(81, 26, 17);

    // I could use three arrays instead of dictionaries... but it doesnt matter
    private static readonly Dictionary<Suit, string> SuitNames = new()
    {
        { Suit.Clubs, "kule" },
        { Suit.Diamonds, "srdce" },
        { Suit.Hearts, "listy" },
        { Suit.Spades, "zaludy" }
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

    static UiCommon()
    {
        foreach (var suite in SuitNames.Keys)
        foreach (var rank in RankMap.Keys)
        {
            var card = new Card(suite, rank);

            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "res",
                $"{SuitNames[suite]}_{RankMap[rank]}.png");
            var image = Image.FromFile(imagePath);

            Images.Add(card, image);
        }

        IsInitialized = true;
    }

    private static Dictionary<Card, Image> Images { get; } = new();

    public static Button InitializeMenuButton(string text, int position, EventHandler clickEvent)
    {
        var button = new Button();

        button.Anchor = AnchorStyles.None;
        button.Size = new Size(200, 50);
        button.Location = new Point(20, 20 + position * 60);

        button.BackColor = UiColor;
        button.FlatAppearance.MouseOverBackColor = UiOverColor;

        button.Font = MenuFont;
        button.ForeColor = Color.FromArgb(255, 255, 255);

        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderSize = 0;

        button.TextAlign = ContentAlignment.MiddleCenter;
        button.Text = text;

        button.Click += clickEvent;

        return button;
    }

    public static Image GetImage(Card card)
    {
        if (IsInitialized) return Images[card];

        try
        {
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "res",
                $"{SuitNames[card.Suit]}_{RankMap[card.Rank]}.png");
            var image = Image.FromFile(imagePath);
            return image;
        }
        catch (Exception)
        {
            throw new Exception("Missing file for card " + card);
        }
    }
}