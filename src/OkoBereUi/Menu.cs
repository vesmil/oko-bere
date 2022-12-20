namespace OkoBereUi;

public partial class Menu : Form
{
    public Menu()
    {
        InitializeComponent();
        BackColor = Color.FromArgb(173, 172, 102);

        var connectButton = UiCommon.InitializeMenuButton("Connect to Server", 0, ConnectButton_Click!);
        Controls.Add(connectButton);

        var playButton = UiCommon.InitializeMenuButton("Single player", 1, PlayButton_Click!);
        Controls.Add(playButton);

        var exitButton = UiCommon.InitializeMenuButton("Exit", 2 , ExitButton_Click!);
        Controls.Add(exitButton);
    }

    private void ConnectButton_Click(object sender, EventArgs e)
    {
        var connectionDialog = new ConnectionDialog();
        
        if (connectionDialog.ShowDialog() == DialogResult.OK)
        {
            var serverIp = connectionDialog.ServerIp;
            var playerName = connectionDialog.PlayerName;
            ConnectToServer(serverIp, playerName);
        }
    }

    private void PlayButton_Click(object sender, EventArgs e)
    {
        var singleDialog = new SettingsDialog();
        if (singleDialog.ShowDialog() == DialogResult.OK)
        {
            StartSinglePlayerGame(singleDialog.NumPlayers);
        }
    }

    private void ExitButton_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void ConnectToServer(string serverIp, string playerName)
    {
        // TODO connect ...
        // var gameTableForm = new GameTableForm();
        // gameTableForm.Show();
    }

    private void StartSinglePlayerGame(int numPlayers)
    {
        // ...
        
        // var gameTableForm = new GameTableForm();
        // gameTableForm.Show();
    }
}