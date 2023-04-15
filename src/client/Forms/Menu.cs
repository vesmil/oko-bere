using System.Diagnostics;
using OkoClient.Client;

namespace OkoClient.Forms;

public partial class Menu : Form
{
    public Menu()
    {
        InitializeComponent();
        BackColor = Color.FromArgb(181, 203, 141);


        var connectButton = UiCommon.InitializeMenuButton("Connect to Server", 0, Width, ConnectButton_Click!);
        Controls.Add(connectButton);

        var exitButton = UiCommon.InitializeMenuButton("Exit", 1, Width, ExitButton_Click!);
        Controls.Add(exitButton);
    }

    private static void ConnectButton_Click(object sender, EventArgs e)
    {
        var connectionDialog = new ConnectionDialog();

        if (connectionDialog.ShowDialog() == DialogResult.OK)
        {
            var serverIp = connectionDialog.ServerIp;
            var serverPort = connectionDialog.ServerPort;
            var playerName = connectionDialog.PlayerName;

            ConnectToServer(serverIp, serverPort, playerName);
        }
    }

    private static void ConnectToServer(string serverIp, int serverPort, string playerName)
    {
        TcpClient client;

        try
        {
            client = new TcpClient(playerName, serverIp, serverPort);
        }
        catch (Exception e)
        {
            Debug.WriteLine(e.Message);
            MessageBox.Show("Could not connect to server");
            return;
        }

        var gameTableForm = new GameTableForm(client);
        gameTableForm.Show();
    }

    private void ExitButton_Click(object sender, EventArgs e)
    {
        Close();
    }
}