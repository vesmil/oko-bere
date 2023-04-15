using OkoClient.Client;
using OkoCommon;
using OkoCommon.Communication;
using Timer = System.Windows.Forms.Timer;

namespace OkoClient.Forms;

/// <summary>
///     Main form for the game.
///     Contains playrs, cards, bank and all the controls.
/// </summary>
public sealed partial class GameTableForm : Form
{
    private Guid PlayerId => client.PlayerId;
    private GameState GameState => client.GameState;
    private readonly IClient client;

    private const int MaxTime = 60;

    private readonly Label balanceLabel = new();
    private readonly Label bankLabel = new();
    private readonly Button betButton = new();
    private readonly Label betLabel = new();
    private readonly TextBox betTextBox = new();
    
    private readonly List<PictureBox> cardBoxes = new();

    
    private readonly Button continueButton = new();
    
    private readonly Panel buttonPanel = new();
    private readonly Button drawButton = new();
    private readonly Button endTurnButton = new();
    private readonly Label noPlayersLabel = new();

    private readonly List<PlayerBox> playerBoxes = new();
    private readonly Timer timer = new();

    private readonly Label topLabel = new();
    private int timeLeft;
    private string timerMessage = "Time left: ";

    public GameTableForm(IClient client)
    {
        InitializeComponent();
        InitializeHandlers();
        
        WindowState = FormWindowState.Maximized;

        this.client = client;
        this.client.MessageReceived += OnMessageReceived;

        InitializeHidden();
        Render();

        SetTurnInfo("Waiting for other players to join...");
        
        Resize += (_, _) =>
        {
            Render();
            UpdateLabels();
        };
    }
    
    private void InitializeHidden()
    {
        continueButton.Size = new Size(200, 50);
        continueButton.Location =
            new Point(Width / 2 - continueButton.Width / 2, Height / 2 - continueButton.Height / 2);
        continueButton.Text = "Join Next Round";
        continueButton.Click += ContinueButton_Click!;

        continueButton.Visible = false;
    }

    private void Render()
    {
        AddTurnInfo();
        AddPlayerBoxes();
        AddMoneyLabels();
        AddCardBoxes();
        AddButtonPanel();
    }

    private void UpdateLabels()
    {
        RefreshPlayerLabels();
        SetLabels();
    }
    
    private void SetLabels()
    {
        balanceLabel.CheckInvoke(() =>
        {
            balanceLabel.Text = "Balance: " + GameState.Players.FirstOrDefault(p => p.Id == PlayerId).Balance;
            bankLabel.Text = "Bank: " + GameState.Bank;
            betLabel.Text = "Bet: " + GameState.Players.FirstOrDefault(p => p.Id == PlayerId).Bet;
        });
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

        balanceLabel.CheckInvoke(() =>
        {
            bankLabel.AutoSize = true;
            bankLabel.Location = new Point(30, 250);
            bankLabel.Font = labelFont;
            bankLabel.Text = "Bank: " + GameState.Bank;
            AddControl(bankLabel);

            betLabel.AutoSize = true;
            betLabel.Location = new Point(30, 290);
            betLabel.Font = labelFont;
            betLabel.Text = "Bet: " + 0;
            AddControl(betLabel);

            balanceLabel.AutoSize = true;
            balanceLabel.Location = new Point(30, 330);
            balanceLabel.Font = labelFont;
            balanceLabel.Text = "Balance: " + 0;
            AddControl(balanceLabel);
        });
    }
    
    private void AddCardBoxes()
    {
        const int cardBoxWidth = 75;
        const int cardBoxHeight = 100;
        const int cardBoxSpacing = 80;
        const int cardBoxStartX = 10;
        
        var cardBoxStartY = Height - cardBoxHeight - 80;
        
        foreach (var cardBox in cardBoxes) cardBox.Dispose();

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
            RenderNoPlayers();
            return;
        }
        
        noPlayersLabel.CheckInvoke(() => noPlayersLabel.Visible = false);
        
        foreach (var box in playerBoxes.Where(box => GameState.Players.All(p => p.Id != box.Player.Id)))
        {
            RemoveControl(box);
            box.Dispose();
        }


        for (var i = 0; i < GameState.Players.Count; i++)
        {
            if (playerBoxes.Any(box => box.Player.Id == GameState.Players[i].Id)) continue;
            
            var player = GameState.Players[i];
            var playerBox = new PlayerBox(player, PlayerId, i);
            playerBox.SelectButton.Click += (_, _) => SelectCutPlayer(player.Id);

            AddControl(playerBox);
            playerBoxes.Add(playerBox);
        }
    }

    private void RenderNoPlayers()
    {
        noPlayersLabel.Visible = true;
        noPlayersLabel.AutoSize = true;
        noPlayersLabel.Location = new Point(30, 70);
        noPlayersLabel.Text = "No players yet";
        AddControl(noPlayersLabel);
    }

    private void RefreshPlayerLabels()
    {
        foreach (var box in playerBoxes)
        {
            box.SetLabels();
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
        
        // TODO
        // Accept button
        // Decline button

        betLabel.AutoSize = true;
        buttonPanel.Location = new Point(Width - buttonPanel.Width - 0, Height - buttonPanel.Height - 50);
        
        AddControl(buttonPanel);
        
        // TODO hide per button
        buttonPanel.Hide();
    }

    private void OnMessageReceived(object? sender, MessageReceivedEventArgs message)
    {
        if (messageHandlers.TryGetValue(message.Type, out var handler))
        {
            handler(message);
        }

        if (NeededInit(message))
        {
            Render();
        }
        
        UpdateLabels();
    }

    private static readonly NotifEnum[] NecessaryInitMessages =
    {
        NotifEnum.NewBanker,
        NotifEnum.NewPlayer,
        NotifEnum.PlayerLeft,
        // ...
    };
    
    private static bool NeededInit(MessageReceivedEventArgs message)
    {
        return NecessaryInitMessages.Contains(message.Type);
    }

    private void ShowContinueButton()
    {
        timerMessage = "Confirm to continue in ";
        continueButton.Show();

        AddControl(continueButton);

        timeLeft = MaxTime;

        timer.Interval = 1000;
        timer.Tick += TimerTick;
        timer.Start();
    }

    private void TimerTick(object? sender, EventArgs e)
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
        // TODO client.Draw();
        buttonPanel.Hide();
    }

    private void BetButton_Click(object sender, EventArgs e)
    {
        // TODO client.Be

        buttonPanel.Hide();
    }

    private void EndTurnButton_Click(object sender, EventArgs e)
    {
        // TODO client.EndTurn();
        
        topLabel.Text = "Next Player's Turn";
        buttonPanel.Hide();
    }

    private void PlaceBet(int amount)
    {
        // Place a bet of the specified amount
        betLabel.Text = $"Bet: ${amount}";
    }
}

public class ButtonBox : Panel
{
    public ButtonBox()
    {
        Size = new Size(75, 23);
        Location = new Point(0, 0);
        
        // TODO ...
    }
}