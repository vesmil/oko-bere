using OkoClient.Client;

namespace OkoClient.Forms;

public partial class Menu : Form
{
    public Menu()
    {
        InitializeComponent();
        BackColor = Color.FromArgb(181, 203, 141);

        var connectButton = UiCommon.InitializeMenuButton("Connect to Server", 0, ConnectButton_Click!);
        Controls.Add(connectButton);

        var exitButton = UiCommon.InitializeMenuButton("Exit", 1, ExitButton_Click!);
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
        var ipAndPort = serverIp.Split(':');
        
        // TODO tryparse
        var client = new TcpClient(playerName, ipAndPort[0], int.Parse(ipAndPort[1]));
        var gameTableForm = new GameTableForm(client);
        
        gameTableForm.Show();
    }
}