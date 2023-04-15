using OkoClient.Client;
using OkoCommon;
using OkoCommon.Communication;
using OkoCommon.Game;
using Timer = System.Windows.Forms.Timer;

namespace OkoClient.Forms;

/// <summary>
///     Main form for the game.
///     Contains playrs, cards, bank and all the controls.
/// </summary>
public sealed partial class GameTableForm : Form
{
    private const int MaxTime = 60;

    /// <summary>
    ///     Messages after which the form has to be rendered again.
    /// </summary>
    private static readonly NotifEnum[] NecessaryInitMessages =
    {
        NotifEnum.NewBanker,
        NotifEnum.NewPlayer,
        NotifEnum.PlayerLeft,
        NotifEnum.ReceivedCard,
        NotifEnum.UpdateGameState
    };

    // Logic
    private readonly IClient client;
    private GameState GameState => client.GameState;
    private Guid PlayerId => client.PlayerId;
    
    // Ui elements
    private readonly ButtonBox buttonPanel = new();
    private readonly List<PictureBox> cardBoxes = new();
    private readonly List<PlayerBox> playerBoxes = new();
    
    // Labels
    private readonly Label noPlayersLabel = new();
    private readonly Label oldTopLabel = new();
    private readonly Label balanceLabel = new();
    private readonly Label bankLabel = new();
    private readonly Label betLabel = new();
    private readonly Label scoreLabel = new();

    // Timing - mostly used for continue and lobby
    private readonly Timer timer = new();
    private readonly Label topLabel = new();
    private int timeLeft;
    private string timerMessage = "Time left: ";

    public GameTableForm(IClient client)
    {
        InitializeComponent();
        InitializeHandlers();

        BackColor = Color.FromArgb(174, 203, 143);

        Size = new Size(1100, 600);
        MinimumSize = new Size(1000, 550);

        // NOTE consider: WindowState = FormWindowState.Maximized;

        this.client = client;
        this.client.MessageReceived += OnMessageReceived;

        Render();
        UpdateLabels();

        SetTurnInfo("Waiting for other players to join...");
        SetButtonPanel();

        Resize += (_, _) =>
        {
            Render();
            UpdateLabels();
        };
    }

    /// <summary>
    ///     Creates all the necessary properties for the form.
    ///     Can be called multiple times to overwrite the old properties.
    /// </summary>
    private void Render()
    {
        buttonPanel.Shift(Width - 10, Height - 50);

        AddTurnInfo();
        AddPlayerBoxes();
        AddMoneyLabels();
        AddCardBoxes();
    }

    /// <summary>
    ///     Calls the appropriate handler for the message from the server.
    ///     If necessary it also calls redraws the entire form.
    /// </summary>
    private void OnMessageReceived(object? sender, MessageReceivedEventArgs message)
    {
        if (messageHandlers.TryGetValue(message.Type, out var handler)) handler(message);

        if (NeededInit(message)) Render();
        UpdateLabels();
    }

    /// <summary>
    ///     Update all written text in the form.
    /// </summary>
    private void UpdateLabels()
    {
        balanceLabel.CheckInvoke(() =>
        {
            bankLabel.Text = "Bank: " + GameState.Bank;
            balanceLabel.Text = "Your balance: " + GameState.GetPlayerInfo(PlayerId).Balance;
            betLabel.Text = "Your bet: " + GameState.GetPlayerInfo(PlayerId).Bet;

            foreach (var box in playerBoxes) box.SetLabels();

            if (GameState.Hand.Count > 0)
            {
                var scoreOptions = GameState.Hand.GetSum();
                scoreLabel.Text = "Score: " + scoreOptions[0];

                for (var i = 1; i < scoreOptions.Count; i++) scoreLabel.Text += " or " + scoreOptions[i];

                scoreLabel.Location = scoreLabel.Location with { X = 10 };
            }
            else
                scoreLabel.Text = "";
        });
    }

    private void SetTurnInfo(string text)
    {
        topLabel.CheckInvoke(() =>
        {
            oldTopLabel.Text = topLabel.Text;
            oldTopLabel.Location = new Point(Width / 2 - oldTopLabel.Size.Width / 2, 15);

            topLabel.Text = text;
            topLabel.Location = new Point(Width / 2 - topLabel.Size.Width / 2, 35);
        });
    }

    private void AddTurnInfo()
    {
        oldTopLabel.CheckInvoke(() =>
        {
            oldTopLabel.ForeColor = Color.FromArgb(110, 140, 110);
            oldTopLabel.AutoSize = true;
            oldTopLabel.Font = new Font("Arial", 8);
            oldTopLabel.Location = new Point(Width / 2 - oldTopLabel.Size.Width / 2, 15);

            topLabel.AutoSize = true;
            topLabel.Font = new Font("Arial", 9);
            topLabel.Location = new Point(Width / 2 - topLabel.Size.Width / 2, 35);
        });

        AddControl(oldTopLabel);
        AddControl(topLabel);
    }

    private void AddMoneyLabels()
    {
        var labelFont = new Font("Arial", 12, FontStyle.Bold);

        balanceLabel.CheckInvoke(() =>
        {
            bankLabel.AutoSize = true;
            bankLabel.Location = new Point(30, 250);
            bankLabel.Font = labelFont;
            AddControl(bankLabel);

            if (!GameState.GetPlayerInfo(PlayerId).IsBanker)
            {
                betLabel.Show();
                balanceLabel.Show();

                betLabel.AutoSize = true;
                betLabel.Location = new Point(30, 290);
                betLabel.Font = labelFont;
                betLabel.Text = "Bet: " + 0;
                AddControl(betLabel);

                balanceLabel.AutoSize = true;
                balanceLabel.Location = new Point(30, 330);
                balanceLabel.Font = labelFont;
                AddControl(balanceLabel);
            }
            else
            {
                betLabel.Hide();
                balanceLabel.Hide();
            }
        });
    }

    private void AddCardBoxes()
    {
        const int cardBoxWidth = 75;
        const int cardBoxHeight = 100;
        const int cardBoxSpacing = 80;
        const int cardBoxStartX = 10;

        var cardBoxStartY = Height - cardBoxHeight - 80;

        foreach (var cardBox in cardBoxes) cardBox.CheckInvoke(() => cardBox.Dispose());

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

        if (GameState.Hand.Count > 0)
            scoreLabel.CheckInvoke(() =>
            {
                scoreLabel.AutoSize = true;
                scoreLabel.Font = new Font("Arial", 10);
                scoreLabel.ForeColor = Color.FromArgb(110, 140, 110);

                scoreLabel.Location = new Point(0, cardBoxStartY - 30);

                AddControl(scoreLabel);
            });
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
            var existingPlayerBox = playerBoxes.FirstOrDefault(box => box.Player.Id == GameState.Players[i].Id);
            if (existingPlayerBox != null)
            {
                existingPlayerBox.Player = GameState.Players[i];
                existingPlayerBox.SetLabels();
                continue;
            }

            var player = GameState.Players[i];
            var playerBox = new PlayerBox(player, i, player.Id == PlayerId);
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

    private void SetButtonPanel()
    {
        buttonPanel.ContinueButton.Click += ContinueButtonClick!;
        buttonPanel.DrawButton.Click += DrawButtonClick!;
        buttonPanel.BetButton.Click += BetButtonClick!;
        buttonPanel.EndTurnButton.Click += EndTurnButtonClick!;
        buttonPanel.AcceptButton.Click += (_, _) => RespondToDuel();
        buttonPanel.DeclineButton.Click += (_, _) => DeclineDuel();
        buttonPanel.ExchangeButton.Click += ExchangeButtonClick;
        buttonPanel.DeclineExchangeButton.Click += DeclineExchangeButtonClick!;
        buttonPanel.MalaDomuButton.Click += MalaDomuButtonClick;
        buttonPanel.DeclineMalaDomuButton .Click += DeclineMalaDomuButtonClick;

        AddControl(buttonPanel);
    }

    private static bool NeededInit(MessageReceivedEventArgs message)
    {
        return NecessaryInitMessages.Contains(message.Type);
    }

    private void ShowContinueButton()
    {
        timerMessage = "Confirm to continue in ";
        
        buttonPanel.CheckInvoke(() =>
        {
            buttonPanel.ContinueButton.Show();
        });

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

            buttonPanel.ContinueButton.Hide();
            SetTurnInfo("See you next time!");
            timer.Stop();
        }
    }

    private void AddControl(Control control)
    {
        if (InvokeRequired)
            Invoke((MethodInvoker)delegate { Controls.Add(control); });
        else
            Controls.Add(control);
    }

    private void RemoveControl(Control control)
    {
        if (InvokeRequired)
            Invoke((MethodInvoker)delegate { Controls.Remove(control); });
        else
            Controls.Remove(control);
    }
}