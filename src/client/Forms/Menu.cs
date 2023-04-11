using OkoClient.Client;

namespace OkoClient.Forms;

public partial class Menu : Form
{
    public Menu()
    {
        InitializeComponent();
        BackColor = Color.FromArgb(173, 172, 102);

        var connectButton = UiCommon.InitializeMenuButton("Connect to Server", 0, ConnectButton_Click!);
        Controls.Add(connectButton);

        var exitButton = UiCommon.InitializeMenuButton("Exit", 2, ExitButton_Click!);
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
    
    private void ExitButton_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void ConnectToServer(string serverIp, string playerName)
    {
        var client = new TcpClient(playerName, serverIp, 1234);
        var gameTableForm = new GameTableForm(client);
        gameTableForm.Show();
    }
}