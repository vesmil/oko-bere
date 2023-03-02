namespace OkoClient.Forms;

public partial class SettingsDialog : Form
{
    private readonly TrackBar numPlayersSlider = new();

    public SettingsDialog()
    {
        InitializeComponent();

        Size = new Size(300, 200);
        CenterToParent();

        var numPlayersLabel = new Label();
        numPlayersLabel.Size = new Size(170, 30);
        numPlayersLabel.Location = new Point(50, 10);
        numPlayersLabel.Text = "Number of players:";
        Controls.Add(numPlayersLabel);

        numPlayersSlider.Size = new Size(200, 30);
        numPlayersSlider.Location = new Point((Width - numPlayersSlider.Width) / 2, 50);
        numPlayersSlider.Minimum = 2;
        numPlayersSlider.Maximum = 14;
        Controls.Add(numPlayersSlider);

        var currentNumPlayersLabel = new Label();
        currentNumPlayersLabel.Size = new Size(200, 30);
        currentNumPlayersLabel.Location = new Point(numPlayersLabel.Location.X + numPlayersLabel.Width + 10, 10);
        currentNumPlayersLabel.Text = numPlayersSlider.Value.ToString();
        Controls.Add(currentNumPlayersLabel);

        numPlayersSlider.ValueChanged += (_, _) => { currentNumPlayersLabel.Text = numPlayersSlider.Value.ToString(); };

        var okButton = new Button();
        okButton.Size = new Size(200, 40);
        okButton.Location = new Point((Width - okButton.Width) / 2, 110);
        okButton.Text = "OK";
        okButton.Click += OkButton_Click!;
        Controls.Add(okButton);
    }

    public int NumPlayers { get; private set; } = 5;

    private void OkButton_Click(object? sender, EventArgs? e)
    {
        NumPlayers = numPlayersSlider.Value;

        DialogResult = DialogResult.OK;
        Close();
    }
}