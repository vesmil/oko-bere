namespace OkoClient.Forms;

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
        ContinueButton.Size = new Size(200, 50);
        ContinueButton.Location = new Point(0, 0);
        ContinueButton.Text = "Join Next Round";

        DrawButton.Size = new Size(75, 23);
        DrawButton.Location = new Point(0, 0);
        DrawButton.Text = "Draw";

        BetButton.Size = new Size(75, 23);
        BetButton.Location = new Point(0, 30);
        BetButton.Text = "Bet";

        BetTextBox.Size = new Size(75, 23);
        BetTextBox.Location = new Point(80, 30);
        BetTextBox.Text = "0";

        EndTurnButton.Size = new Size(75, 23);
        EndTurnButton.Location = new Point(0, 60);
        EndTurnButton.Text = "End Turn";

        AcceptButton.Size = new Size(75, 23);
        AcceptButton.Location = new Point(0, 30);
        AcceptButton.Text = "Accept";

        DeclineButton.Size = new Size(75, 23);
        DeclineButton.Location = new Point(0, 60);
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