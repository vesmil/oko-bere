namespace OkoClient.Forms;

/// <summary>
///     Panel containing all buttons with simple methods for showing/hiding them.
/// </summary>
public sealed class ButtonBox : Panel
{
    public readonly Button AcceptButton = new();
    public readonly Button BetButton = new();
    public readonly TextBox BetTextBox = new();

    public readonly Button ContinueButton = new();
    public readonly Button DeclineButton = new();
    public readonly Button DrawButton = new();

    public readonly Button EndTurnButton = new();

    public ButtonBox()
    {
        var buttonSize = new Size(75, 23);

        ContinueButton.Size = new Size(200, 50);
        ContinueButton.Location = new Point(0, 0);
        ContinueButton.Text = "Join Next Round";

        BetButton.Size = buttonSize;
        BetButton.Location = new Point(0, 0);
        BetButton.Text = "Bet";

        BetTextBox.Size = buttonSize;
        BetTextBox.Location = new Point(80, 0);
        BetTextBox.Text = "0";
        BetTextBox.KeyPress += (sender, args) =>
        {
            if (!char.IsControl(args.KeyChar) && !char.IsDigit(args.KeyChar))
                args.Handled = true;
        };

        DrawButton.Size = buttonSize;
        DrawButton.Location = new Point(0, 30);
        DrawButton.Text = "Draw";

        EndTurnButton.Size = buttonSize;
        EndTurnButton.Location = new Point(0, 60);
        EndTurnButton.Text = "End Turn";

        AcceptButton.Size = buttonSize;
        AcceptButton.Location = new Point(0, 0);
        AcceptButton.Text = "Accept";

        DeclineButton.Size = buttonSize;
        DeclineButton.Location = new Point(0, 30);
        DeclineButton.Text = "Decline";

        AutoSize = true;

        Controls.Add(DrawButton);
        Controls.Add(BetButton);
        Controls.Add(BetTextBox);
        Controls.Add(EndTurnButton);
        Controls.Add(AcceptButton);
        Controls.Add(DeclineButton);
        Controls.Add(ContinueButton);

        HideAll();
    }

    public void Shift(int x, int y)
    {
        Location = new Point(x - Width, y - Height);
    }

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

    public void Duel()
    {
        AcceptButton.CheckInvoke(() =>
        {
            AcceptButton.Show();
            DeclineButton.Show();
            BetTextBox.Show();
        });
    }

    public void NoBet()
    {
        DrawButton.CheckInvoke(() =>
        {
            DrawButton.Show();
            EndTurnButton.Show();
        });
    }

    public void Continue()
    {
        ContinueButton.CheckInvoke(() => { ContinueButton.Show(); });
    }

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
        });
    }
}