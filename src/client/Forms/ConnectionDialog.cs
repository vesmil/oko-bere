using OkoClient.Client;

namespace OkoClient.Forms;

public partial class ConnectionDialog : Form
{
    private readonly TextBox playerNameTextBox;
    private readonly TextBox serverIpTextBox;

    public ConnectionDialog()
    {
        InitializeComponent();

        Size = new Size(300, 200);
        CenterToParent();

        serverIpTextBox = new TextBox();
        serverIpTextBox.Size = new Size(200, 30);
        serverIpTextBox.Location = new Point((Width - serverIpTextBox.Width) / 2, 10);
        Controls.Add(serverIpTextBox);

        playerNameTextBox = new TextBox();
        playerNameTextBox.Size = new Size(200, 30);
        playerNameTextBox.Location = new Point((Width - playerNameTextBox.Width) / 2, 40);
        Controls.Add(playerNameTextBox);

        serverIpTextBox.PlaceholderText("ServerIP:port");
        playerNameTextBox.PlaceholderText("Player name");

        var connectButton = new Button();
        connectButton.Size = new Size(200, 40);
        connectButton.Location = new Point((Width - connectButton.Width) / 2, 70);
        connectButton.Text = "Connect";
        connectButton.Click += ConnectButton_Click!;
        Controls.Add(connectButton);

        DialogResult = DialogResult.Cancel;
    }

    public string ServerIp { get; private set; } = "";
    public string PlayerName { get; private set; } = "";

    private void ConnectButton_Click(object sender, EventArgs e)
    {
        ServerIp = serverIpTextBox.Text;
        PlayerName = playerNameTextBox.Text;
        DialogResult = DialogResult.OK;

        Close();
    }
}

public static class WinformExtensions
{
    public static void PlaceholderText(this TextBox textBox, string text)
    {
        textBox.Text = text;
        textBox.ForeColor = Color.Gray;

        textBox.GotFocus += (_, _) =>
        {
            if (textBox.Text == text)
            {
                textBox.Text = "";
                textBox.ForeColor = Color.Black;
            }
        };

        textBox.LostFocus += (_, _) =>
        {
            if (textBox.Text == "")
            {
                textBox.Text = text;
                textBox.ForeColor = Color.Gray;
            }
        };
    }
}