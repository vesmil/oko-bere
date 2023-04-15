namespace OkoClient.Forms;

/// <summary>
///     Dialog for specifying server IP, port and player name.
/// </summary>
public partial class ConnectionDialog : Form
{
    private readonly TextBox playerNameTextBox;
    private readonly TextBox serverIpTextBox;
    private readonly TextBox serverPortTextBox;

    public ConnectionDialog()
    {
        InitializeComponent();

        Size = new Size(300, 200);
        CenterToParent();

        serverIpTextBox = new TextBox();
        serverIpTextBox.Size = new Size(200, 30);
        serverIpTextBox.Location = new Point((Width - serverIpTextBox.Width) / 2, 10);
        Controls.Add(serverIpTextBox);

        serverPortTextBox = new TextBox();
        serverPortTextBox.Size = new Size(200, 30);
        serverPortTextBox.Location = new Point((Width - serverPortTextBox.Width) / 2, 40);
        Controls.Add(serverPortTextBox);

        playerNameTextBox = new TextBox();
        playerNameTextBox.Size = new Size(200, 30);
        playerNameTextBox.Location = new Point((Width - playerNameTextBox.Width) / 2, 70);
        Controls.Add(playerNameTextBox);

        serverIpTextBox.PlaceholderText("Server IP");
        serverPortTextBox.PlaceholderText("Server port");
        playerNameTextBox.PlaceholderText("Player name");

        var connectButton = new Button();
        connectButton.Size = new Size(200, 40);
        connectButton.Location = new Point((Width - connectButton.Width) / 2, 110);
        connectButton.Text = "Connect";
        connectButton.Click += ConnectButton_Click!;
        Controls.Add(connectButton);

        DialogResult = DialogResult.Cancel;
    }

    public string ServerIp { get; private set; } = "";
    public string PlayerName { get; private set; } = "";
    public int ServerPort { get; private set; }

    /// <summary>
    ///     Connect button click handler.
    ///     Checks if IP, port and player name are valid and closes the dialog with DialogResult.OK.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ConnectButton_Click(object sender, EventArgs e)
    {
        ServerIp = serverIpTextBox.Text;
        if (ServerIp == "")
        {
            MessageBox.Show("Invalid IP");
            return;
        }

        if (int.TryParse(serverPortTextBox.Text, out var port))
        {
            ServerPort = port;
        }
        else
        {
            MessageBox.Show("Invalid port");
            return;
        }
        
        PlayerName = playerNameTextBox.Text;
        if (PlayerName == "")
        {
            MessageBox.Show("Invalid player name");
            return;
        }
        
        DialogResult = DialogResult.OK;

        Close();
    }
}