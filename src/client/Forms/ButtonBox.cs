namespace OkoClient.Forms;

/// <summary>
///     Panel containing all buttons with simple methods for showing/hiding them.
/// </summary>
public sealed class ButtonBox : Panel
{
    // Note Those button could probably be repurposed - this is kinda bad design
    
    public readonly Button AcceptButton = new();
    public readonly Button BetButton = new();
    public readonly TextBox BetTextBox = new();

    public readonly Button ContinueButton = new();
    public readonly Button DeclineButton = new();
    public readonly Button DrawButton = new();
    
    public readonly Button ExchangeButton = new();
    public readonly Button DeclineExchangeButton = new();
    
    public readonly Button MalaDomuButton = new();
    public readonly Button DeclineMalaDomuButton = new();
    
    public readonly Button EndTurnButton = new();

    private static readonly Size ButtonSize = new(90, 23);
    
    public ButtonBox()
    {
        ContinueButton.Size = new Size(180, 40);
        ContinueButton.Location = new Point(0, 0);
        ContinueButton.Text = "Join Next Round";

        ExchangeButtons();
        TurnButtons();
        DuelButtons();
        MalaDomuButtons();
        
        AutoSize = true;
        
        Controls.Add(DrawButton);
        Controls.Add(BetButton);
        Controls.Add(BetTextBox);
        Controls.Add(EndTurnButton);
        Controls.Add(AcceptButton);
        Controls.Add(DeclineButton);
        Controls.Add(ContinueButton);
        Controls.Add(ExchangeButton);
        Controls.Add(DeclineExchangeButton);
        Controls.Add(MalaDomuButton);
        Controls.Add(DeclineMalaDomuButton);

        HideAll();
    }

    private void MalaDomuButtons()
    {
        MalaDomuButton.Size = ButtonSize;
        MalaDomuButton.Location = new Point(0, 0);
        MalaDomuButton.Text = "Mala Domu";
        
        DeclineMalaDomuButton.Size = ButtonSize;
        DeclineMalaDomuButton.Location = new Point(0, 30);
        DeclineMalaDomuButton.Text = "Nah";
    }

    private void DuelButtons()
    {
        AcceptButton.Size = ButtonSize;
        AcceptButton.Location = new Point(0, 0);
        AcceptButton.Text = "Accept";

        DeclineButton.Size = ButtonSize;
        DeclineButton.Location = new Point(0, 30);
        DeclineButton.Text = "Decline";
    }

    private void TurnButtons()
    {
        BetButton.Size = ButtonSize;
        BetButton.Location = new Point(0, 0);
        BetButton.Text = "Bet";

        BetTextBox.Size = ButtonSize;
        BetTextBox.Location = new Point(95, 0);
        BetTextBox.Text = "0";
        BetTextBox.KeyPress += (_, args) =>
        {
            if (!char.IsControl(args.KeyChar) && !char.IsDigit(args.KeyChar))
                args.Handled = true;
        };

        DrawButton.Size = ButtonSize;
        DrawButton.Location = new Point(0, 30);
        DrawButton.Text = "Draw";

        EndTurnButton.Size = ButtonSize;
        EndTurnButton.Location = new Point(0, 60);
        EndTurnButton.Text = "End Turn";
    }

    private void ExchangeButtons()
    {
        ExchangeButton.Size = ButtonSize;
        ExchangeButton.Location = new Point(0, 0);
        ExchangeButton.Text = "Exchange";
        
        DeclineExchangeButton.Size = ButtonSize;
        DeclineExchangeButton.Location = new Point(0, 30);
        DeclineExchangeButton.Text = "Decline Exchange";
    }

    /// <summary>
    ///     Move the panel to the specified location
    /// </summary>
    public void Shift(int x, int y)
    {
        Location = new Point(x - Width, y - Height);
    }

    /// <summary>
    ///     Show all buttons for a turn
    /// </summary>
    public void Turn()
    {
        DrawButton.CheckInvoke(() =>
        {
            DrawButton.Show();
            BetButton.Show();
            BetTextBox.Show();
            EndTurnButton.Show();
        });
    }
    
    /// <summary>
    ///     Show all buttons to allow the player to exchange cards
    /// </summary>
    public void Exchange()
    {
        ExchangeButton.CheckInvoke(() =>
        {
            ExchangeButton.Show();
            DeclineExchangeButton.Show();
        });
    }

    /// <summary>
    ///     Show all buttons for a duel
    /// </summary>
    public void Duel()
    {
        AcceptButton.CheckInvoke(() =>
        {
            AcceptButton.Show();
            DeclineButton.Show();
            BetTextBox.Show();
        });
    }

    /// <summary>
    ///     Show all buttons for a turn without a bet - usually duel or banker's turn
    /// </summary>
    public void NoBet()
    {
        DrawButton.CheckInvoke(() =>
        {
            DrawButton.Show();
            EndTurnButton.Show();
        });
    }

    /// <summary>
    ///     Ask the player if they want to continue
    /// </summary>
    public void Continue()
    {
        ContinueButton.CheckInvoke(() => { ContinueButton.Show(); });
    }

    /// <summary>
    ///     Hide all buttons
    /// </summary>
    public void HideAll()
    {
        DrawButton.CheckInvoke(() =>
        {
            DrawButton.Hide();
            BetButton.Hide();
            BetTextBox.Hide();
            EndTurnButton.Hide();
            AcceptButton.Hide();
            DeclineButton.Hide();
            ContinueButton.Hide();
            ExchangeButton.Hide();
            DeclineExchangeButton.Hide();
            MalaDomuButton.Hide();
            DeclineMalaDomuButton.Hide();
        });
    }

    /// <summary>
    ///     Show all buttons for a mala domu call
    /// </summary>
    public void MalaDomu()
    {
        MalaDomuButton.CheckInvoke(() =>
        {
            MalaDomuButton.Show();
            DeclineMalaDomuButton.Show();
        });
    }
}