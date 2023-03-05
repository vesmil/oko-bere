using OkoClient.Client;
using OkoCommon;
using Timer = System.Windows.Forms.Timer;

namespace OkoClient.Forms;

/// <summary>
///     Main form for the game.
///     Contains playrs, cards, bank and all the controls.
/// </summary>
public sealed partial class GameTableForm : Form
{
    private const int MaxTime = 60;

    private readonly Label balanceLabel = new();
    private readonly Label bankLabel = new();
    private readonly Button betButton = new();
    private readonly Label betLabel = new();
    private readonly TextBox betTextBox = new();
    private readonly Panel buttonPanel = new();

    private readonly List<PictureBox> cardBoxes = new();
    private readonly IClient client;

    private readonly Button continueButton = new();

    private readonly Button drawButton = new();
    private readonly Button endTurnButton = new();
    private readonly Label noPlayersLabel = new();

    private readonly List<GroupBox> playerBoxes = new();
    private readonly Timer timer = new();

    private readonly Label topLabel = new();
    private int timeLeft;
    private string timerMessage = "Time left: ";

    public GameTableForm(IClient client)
    {
        InitializeComponent();
        InitializeHandlers();
        
        // TODO in final WindowState = FormWindowState.Maximized;

        this.client = client;
        this.client.MessageReceived += OnMessageReceived;

        InitializeHidden();
        InitializeVisible();

        SetTurnInfo("Waiting for other players to join...");
    }

    private GameState GameState => client.GameState;

    private void InitializeHidden()
    {
        continueButton.Size = new Size(200, 50);
        continueButton.Location =
            new Point(Width / 2 - continueButton.Width / 2, Height / 2 - continueButton.Height / 2);
        continueButton.Text = "Join Next Round";
        continueButton.Click += ContinueButton_Click!;

        continueButton.Visible = false;
    }

    private void InitializeVisible()
    {
        AddTurnInfo();
        AddPlayerBoxes();
        AddMoneyLabels();
        AddCardBoxes();
        AddButtonPanel();
    }

    private void Render()
    {
        AddPlayerBoxes();
    }

    private void AddControl(Control control)
    {
        if (InvokeRequired)
            Invoke((MethodInvoker) delegate { Controls.Add(control); });
        else
            Controls.Add(control);
    }
    
    private void RemoveControl(Control control)
    {
        if (InvokeRequired)
            Invoke((MethodInvoker) delegate { Controls.Remove(control); });
        else
            Controls.Remove(control);    
    }

    private void AddTurnInfo()
    {
        BackColor = Color.FromArgb(174, 203, 143);
        topLabel.AutoSize = true;
        AddControl(topLabel);
    }

    private void SetTurnInfo(string text)
    {
        topLabel.CheckInvoke(() =>
        {
            topLabel.Text = text;
            topLabel.Location = new Point(Width / 2 - topLabel.Size.Width / 2, 20);
        });
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
        if (GameState.Players.Count == 0)
        {
            noPlayersLabel.Visible = true;
            noPlayersLabel.AutoSize = true;
            noPlayersLabel.Location = new Point(30, 70);
            noPlayersLabel.Text = "No players yet";
            AddControl(noPlayersLabel);
            return;
        }
        
        noPlayersLabel.CheckInvoke(() => noPlayersLabel.Visible = false);
        
        if (playerBoxes.Count > 0)
        {
            playerBoxes[0].CheckInvoke(() =>
            {
                foreach (var playerBox in playerBoxes) playerBox.Dispose();
                playerBoxes.Clear();
            });
        }

        for (var i = 0; i < GameState.Players.Count; i++)
        {
            var player = GameState.Players[i];

            var playerBox = new GroupBox();
            playerBox.Size = new Size(200, 130);
            playerBox.Location = new Point(30 + 210 * i, 70);
            playerBox.Text = $"{player.Name} {(player.IsBanker ? "(Banker)" : "(Player)")}";
            
            var cardCountLabel = new Label();
            cardCountLabel.AutoSize = true;
            cardCountLabel.Location = new Point(10, 30);
            cardCountLabel.Text = $"Cards: {player.CardCount}";
            playerBox.Controls.Add(cardCountLabel);
            
            var balancePlayerLabel = new Label();
            balancePlayerLabel.AutoSize = true;
            balancePlayerLabel.Location = new Point(10, 60);
            balancePlayerLabel.Text = $"Balance: {player.Balance}";
            playerBox.Controls.Add(balancePlayerLabel);

            if (!player.IsBanker)
            {
                var betPlayerLabel = new Label();
                betPlayerLabel.AutoSize = true;
                betPlayerLabel.Location = new Point(10, 90);
                betPlayerLabel.Text = $"Bet: {player.Bet}";
                playerBox.Controls.Add(betPlayerLabel);
            }
            else
            {
                playerBox.BackColor = Color.FromArgb(64, 255, 255, 128);
            }

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

        // TODO AddControl(buttonPanel);
        buttonPanel.Hide();
    }

    private void OnMessageReceived(object? sender, MessageReceivedEventArgs message)
    {
        if (messageHandlers.TryGetValue(message.Type, out var handler))
        {
            handler(message);
        }

        Render();
    }
    
    private void ShowContinueButton()
    {
        timerMessage = "Confirm to continue in ";

        continueButton.Show();

        AddControl(continueButton);

        timeLeft = MaxTime;

        timer.Interval = 1000;
        timer.Tick += timer_Tick;
        timer.Start();
    }

    private void timer_Tick(object? sender, EventArgs e)
    {
        if (timeLeft > 0)
        {
            timeLeft--;
            SetTurnInfo(timerMessage + timeLeft);
        }
        else
        {
            client.Continue(false);

            continueButton.Hide();
            SetTurnInfo("See you next time!");
            timer.Stop();
        }
    }

    private void ContinueButton_Click(object sender, EventArgs e)
    {
        client.Continue(true);

        continueButton.Hide();
        SetTurnInfo("Waiting for other players...");
        timer.Stop();
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
        topLabel.Text = "Next Player's Turn";
    }

    private void PlaceBet(int amount)
    {
        // Place a bet of the specified amount
        betLabel.Text = $"Bet: ${amount}";
    }
}