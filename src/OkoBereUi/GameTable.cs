using OkoClient;
using OkoCommon.Communication;

namespace OkoBereUi;

public sealed partial class GameTableForm : Form
{
    private readonly Client client;

    private readonly Label playerTurnLabel = new();

    private readonly Label balanceLabel = new();
    private readonly Label bankLabel = new();
    private readonly Button betButton = new();
    private readonly Label betLabel = new();
    private readonly TextBox betTextBox = new();
    private readonly Panel buttonPanel = new();

    private readonly Button drawButton = new();
    private readonly Button endTurnButton = new();

    private readonly List<GroupBox> playerBoxes = new();
    private readonly List<PictureBox> cardBoxes = new();

    // TODO handle the constants
    public GameTableForm(Client client)
    {
        InitializeComponent();
        WindowState = FormWindowState.Maximized;

        this.client = client;

        AddTurnInfo();
        
        AddPlayerBoxes(5);
        
        AddMoneyLabels();
        
        AddCardBoxes(5);
        
        AddButtonPanel();
    }

    private void AddTurnInfo()
    {
        BackColor = Color.FromArgb(174, 203, 143);
        playerTurnLabel.AutoSize = true;
        playerTurnLabel.Location = new Point(Width / 2 - playerTurnLabel.Size.Width / 2, 20);
        playerTurnLabel.Text = "Wait";
        Controls.Add(playerTurnLabel);    
    }
    private void AddMoneyLabels()
    {
        var labelFont = new Font("Arial", 12, FontStyle.Bold);

        bankLabel.AutoSize = true;
        bankLabel.Location = new Point(30, 250);
        bankLabel.Font = labelFont;
        bankLabel.Text = "Bank: ";
        Controls.Add(bankLabel);

        betLabel.AutoSize = true;
        betLabel.Location = new Point(30, 290);
        betLabel.Font = labelFont;
        betLabel.Text = "Bet: ";
        Controls.Add(betLabel);

        balanceLabel.AutoSize = true;
        balanceLabel.Location = new Point(30, 330);
        balanceLabel.Font = labelFont;
        balanceLabel.Text = "Balance: ";
        Controls.Add(balanceLabel);
    }
    private void AddCardBoxes(int count)
    {
        const int cardBoxWidth = 75;
        const int cardBoxHeight = 100;
        const int cardBoxSpacing = 80;
        const int cardBoxStartX = 10;
        const int cardBoxStartY = 360;

        for (var i = 0; i < count; i++)
        {
            var cardBox = new PictureBox();
            cardBox.Size = new Size(cardBoxWidth, cardBoxHeight);
            cardBox.Location = new Point(cardBoxStartX + cardBoxSpacing * i, cardBoxStartY);
            cardBox.SizeMode = PictureBoxSizeMode.StretchImage;
            Controls.Add(cardBox);
            cardBoxes.Add(cardBox);
        }    
    }
    private void AddPlayerBoxes(int count)
    {
        for (var i = 0; i < count; i++)
        {
            var playerBox = new GroupBox();
            playerBox.Size = new Size(200, 130);
            playerBox.Location = new Point(30 + 210 * i, 70);
            playerBox.Text = $"Player {i + 1}";
            
            var balancePlayerLabel = new Label();
            balancePlayerLabel.AutoSize = true;
            balancePlayerLabel.Location = new Point(10, 30);
            balancePlayerLabel.Text = "Balance: 1000";
            playerBox.Controls.Add(balancePlayerLabel);
            
            var betPlayerLabel = new Label();
            betPlayerLabel.AutoSize = true;
            betPlayerLabel.Location = new Point(10, 60);
            betPlayerLabel.Text = "Bet: 0";
            playerBox.Controls.Add(betPlayerLabel);
            
            var cardCountLabel = new Label();
            cardCountLabel.AutoSize = true;
            cardCountLabel.Location = new Point(10, 90);
            cardCountLabel.Text = "Cards: 0";
            playerBox.Controls.Add(cardCountLabel);

            Controls.Add(playerBox);
            playerBoxes.Add(playerBox);
        }
    }
    private void AddButtonPanel()
    {
        drawButton.Size = new Size(75, 23);
        drawButton.Location = new Point(0, 0);
        drawButton.Text = "Draw";
        drawButton.Click += DrawButton_Click!;

        betButton.Size = new Size(75, 23);
        betButton.Location = new Point(0, 30);
        betButton.Text = "Bet";
        betButton.Click += BetButton_Click!;

        betTextBox.Size = new Size(75, 23);
        betTextBox.Location = new Point(80, 30);
        betTextBox.Text = "0";

        endTurnButton.Size = new Size(75, 23);
        endTurnButton.Location = new Point(0, 60);
        endTurnButton.Text = "End Turn";
        endTurnButton.Click += EndTurnButton_Click!;

        buttonPanel.Controls.Add(drawButton);
        buttonPanel.Controls.Add(betButton);
        buttonPanel.Controls.Add(betTextBox);
        buttonPanel.Controls.Add(endTurnButton);
        
        betLabel.AutoSize = true;
        buttonPanel.Location = new Point(Width - buttonPanel.Width - 0, Height - buttonPanel.Height - 50);
        
        Controls.Add(buttonPanel);
        buttonPanel.Hide();
    }

    private void OnMessageReceived(object sender, MessageReceivedEventArgs message)
    {
        // if (messa)

        // If cut request...

        // If new player, new banker, bank busted...

        // ...
    }

    private void DrawButton_Click(object sender, EventArgs e)
    {
        // Send request to server
        // client...

        // Receive response from server
    }

    private void BetButton_Click(object sender, EventArgs e)
    {
        // Send request to server
        // 

        // Receive response from server
    }

    private void EndTurnButton_Click(object sender, EventArgs e)
    {
        client.SendGenericResponse(PlayerResponseEnum.Stop);
        playerTurnLabel.Text = "Next Player's Turn";
    }

    private void PlaceBet(int amount)
    {
        // Place a bet of the specified amount
        betLabel.Text = $"Bet: ${amount}";
    }
}