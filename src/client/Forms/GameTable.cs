using OkoCommon;
using OkoCommon.Communication;

namespace OkoClient.Forms;

public sealed partial class GameTableForm : Form
{
    private readonly Label balanceLabel = new();
    private readonly Label bankLabel = new();
    private readonly Button betButton = new();
    private readonly Label betLabel = new();
    private readonly TextBox betTextBox = new();
    private readonly Panel buttonPanel = new();
    private readonly List<PictureBox> cardBoxes = new();
    private readonly ClientLogics clientLogics;

    private readonly Button drawButton = new();
    private readonly Button endTurnButton = new();

    private readonly List<GroupBox> playerBoxes = new();

    private readonly Label playerTurnLabel = new();

    public GameTableForm(ClientLogics client)
    {
        InitializeComponent();
        // WindowState = FormWindowState.Maximized;

        clientLogics = client;
        clientLogics.MessageReceived += OnMessageReceived;

        Render();
    }

    private GameState GameState => clientLogics.GameState;

    private void Render()
    {
        AddTurnInfo();
        AddPlayerBoxes();
        AddMoneyLabels();
        AddCardBoxes();
        AddButtonPanel();
    }

    private void AddControl(Control control)
    {
        if (InvokeRequired)
        {
            Invoke((MethodInvoker)delegate { Controls.Add(control); });
        }
        else
        {
            Controls.Add(control);
        }
    }
    
    private void AddTurnInfo()
    {
        BackColor = Color.FromArgb(174, 203, 143);
        playerTurnLabel.AutoSize = true;

        if (playerTurnLabel.InvokeRequired)
        {
            playerTurnLabel.Invoke((MethodInvoker)delegate
            {
                playerTurnLabel.Location = new Point(Width / 2 - playerTurnLabel.Size.Width / 2, 20);
            });
        }
        else
        {
            playerTurnLabel.Location = new Point(Width / 2 - playerTurnLabel.Size.Width / 2, 20);
        }
        
        playerTurnLabel.Text = "Wait";
        AddControl(playerTurnLabel);
    }

    private void AddMoneyLabels()
    {
        var labelFont = new Font("Arial", 12, FontStyle.Bold);

        bankLabel.AutoSize = true;
        bankLabel.Location = new Point(30, 250);
        bankLabel.Font = labelFont;
        bankLabel.Text = "Bank: ";
        AddControl(bankLabel);

        betLabel.AutoSize = true;
        betLabel.Location = new Point(30, 290);
        betLabel.Font = labelFont;
        betLabel.Text = "Bet: ";
        AddControl(betLabel);

        balanceLabel.AutoSize = true;
        balanceLabel.Location = new Point(30, 330);
        balanceLabel.Font = labelFont;
        balanceLabel.Text = "Balance: ";
        AddControl(balanceLabel);
    }

    private void AddCardBoxes()
    {
        const int cardBoxWidth = 75;
        const int cardBoxHeight = 100;
        const int cardBoxSpacing = 80;
        const int cardBoxStartX = 10;
        const int cardBoxStartY = 360;

        for (var i = 0; i < GameState.Hand.Count; i++)
        {
            var cardBox = new PictureBox();
            cardBox.Size = new Size(cardBoxWidth, cardBoxHeight);
            cardBox.Location = new Point(cardBoxStartX + cardBoxSpacing * i, cardBoxStartY);
            cardBox.SizeMode = PictureBoxSizeMode.StretchImage;

            cardBox.Image = UiCommon.GetImage(GameState.Hand[i]);

            AddControl(cardBox);
            cardBoxes.Add(cardBox);
        }
    }

    private void AddPlayerBoxes()
    {
        for (var i = 0; i < GameState.Players.Count; i++)
        {
            var player = GameState.Players[i];

            var playerBox = new GroupBox();
            playerBox.Size = new Size(200, 130);
            playerBox.Location = new Point(30 + 210 * i, 70);
            playerBox.Text = $"{(player.IsBanker ? "Banker" : "Player")} - {player.Name}";

            var balancePlayerLabel = new Label();
            balancePlayerLabel.AutoSize = true;
            balancePlayerLabel.Location = new Point(10, 30);
            balancePlayerLabel.Text = $"Balance: {player.Balance}";
            playerBox.Controls.Add(balancePlayerLabel);

            var betPlayerLabel = new Label();
            betPlayerLabel.AutoSize = true;
            betPlayerLabel.Location = new Point(10, 60);
            betPlayerLabel.Text = $"Bet: {player.Bet}";
            playerBox.Controls.Add(betPlayerLabel);

            var cardCountLabel = new Label();
            cardCountLabel.AutoSize = true;
            cardCountLabel.Location = new Point(10, 90);
            cardCountLabel.Text = $"Cards: {player.CardCount}";
            playerBox.Controls.Add(cardCountLabel);

            AddControl(playerBox);
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

        AddControl(buttonPanel);
        buttonPanel.Hide();
    }

    private void OnMessageReceived(object? sender, MessageReceivedEventArgs message)
    {
        switch (message.Type)
        {
            case NotifEnum.GameStart:
                playerTurnLabel.Text = "Game Started";
                break;

            case NotifEnum.NewBanker:
                break;

            case NotifEnum.NewPlayer:
                break;
            case NotifEnum.SetInitialBank:
                break;
            case NotifEnum.BankBusted:
                break;
            case NotifEnum.AskForTurn:
                break;
            case NotifEnum.ReceivedCard:
                break;
            case NotifEnum.CardsDealt:
                break;
            case NotifEnum.Bust:
                break;
            case NotifEnum.AskForMalaDomu:
                break;
            case NotifEnum.MalaDomuCalled:
                break;
            case NotifEnum.MalaDomuSuccess:
                break;
            case NotifEnum.ChooseCutPlayer:
                break;
            case NotifEnum.ChooseCutPosition:
                break;
            case NotifEnum.SeeCutCard:
                break;
            case NotifEnum.DuelOffer:
                break;
            case NotifEnum.DuelDeclined:
                break;
            case NotifEnum.DuelAccepted:
                break;
            case NotifEnum.DuelAskNextCard:
                break;
            case NotifEnum.AlreadyExchanged:
                break;
            case NotifEnum.ExchangeAllowed:
                break;
            case NotifEnum.PlayerLeft:
                break;
            case NotifEnum.AskForContinue:
                break;
            case NotifEnum.Continue:
                break;
            case NotifEnum.NotEnoughPlayers:
                break;
            case NotifEnum.EndOfGame:
                break;
        }

        Render();
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
        // client.SendGenericResponse(PlayerResponseEnum.Stop);
        playerTurnLabel.Text = "Next Player's Turn";
    }

    private void PlaceBet(int amount)
    {
        // Place a bet of the specified amount
        betLabel.Text = $"Bet: ${amount}";
    }
}