namespace OkoBereUi;

public static class UiCommon
{
    private static readonly Font MenuFont = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point, 204);
    private static readonly Color UiColor = Color.FromArgb(161, 87, 72);
    private static readonly Color UiOverColor = Color.FromArgb(81, 26, 17);

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
}