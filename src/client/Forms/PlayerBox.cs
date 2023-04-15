using OkoCommon;

namespace OkoClient.Forms;

public sealed class PlayerBox : GroupBox
{
    private readonly Label balancePlayerLabel = new();
    private readonly Label betPlayerLabel = new();
    private readonly Label cardCountLabel = new();
    private readonly PlayerInfo player;

    public readonly Button SelectButton = new();

    public PlayerBox(PlayerInfo player, Guid playerId, int i)
    {
        this.player = player;

        Size = new Size(200, 130);
        Location = new Point(30 + 210 * i, 70);

        Text = $"{player.Name} {(player.Id == playerId ? "(You)" : "")}" +
               $"\n{(player.IsBanker ? "Banker" : "Player")}";

        cardCountLabel.AutoSize = true;
        cardCountLabel.Location = new Point(10, 40);
        Controls.Add(cardCountLabel);
        
        if (!player.IsBanker)
        {
            balancePlayerLabel.AutoSize = true;
            balancePlayerLabel.Location = new Point(10, 70);
            Controls.Add(balancePlayerLabel);
        
            betPlayerLabel.AutoSize = true;
            betPlayerLabel.Location = new Point(10, 100);
            Controls.Add(betPlayerLabel);
        }
        else
            BackColor = Color.FromArgb(64, 255, 255, 128);

        SelectButton.Size = new Size(75, 40);
        SelectButton.Location = new Point(Size.Width - SelectButton.Size.Width - 10,
            Size.Height - SelectButton.Size.Height - 10);
        SelectButton.Text = "Select";

        Controls.Add(SelectButton);
        SelectButton.Hide();

        SetLabels();
    }

    public PlayerInfo Player => player;

    public void SetLabels()
    {
        cardCountLabel.Text = $"Cards: {player.CardCount}";
        balancePlayerLabel.Text = $"Balance: {player.Balance}";
        betPlayerLabel.Text = $"Bet: {player.Bet}";
    }
}