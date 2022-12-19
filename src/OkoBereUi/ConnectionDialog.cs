namespace OkoBereUi;

public partial class ConnectionDialog : Form
{
    public string ServerIp { get; private set; }= "";
    public string PlayerName { get; private set; } = "";
    
    private readonly TextBox serverIpTextBox;
    private readonly TextBox playerNameTextBox;
    
    public ConnectionDialog()
    {
        InitializeComponent();

        Size = new Size(300, 200);
        CenterToParent();

        serverIpTextBox = new TextBox();
        serverIpTextBox.Size = new Size(200, 23);
        serverIpTextBox.Location = new Point((Width - serverIpTextBox.Width) / 2, 10);
        Controls.Add(serverIpTextBox);

        playerNameTextBox = new TextBox();
        playerNameTextBox.Size = new Size(200, 23);
        playerNameTextBox.Location = new Point((Width - playerNameTextBox.Width) / 2, 40);
        Controls.Add(playerNameTextBox);

        var connectButton = new Button();
        connectButton.Size = new Size(200, 23);
        connectButton.Location = new Point((Width - connectButton.Width) / 2, 70);
        connectButton.Text = "Connect";
        connectButton.Click += ConnectButton_Click!;
        Controls.Add(connectButton);

        DialogResult = DialogResult.Cancel;
    }

    private void ConnectButton_Click(object sender, EventArgs e)
    {
        ServerIp = serverIpTextBox.Text;
        PlayerName = playerNameTextBox.Text;

        // TODO try to connect
        DialogResult = DialogResult.OK;
        // else...
        
        Close();
    }
}