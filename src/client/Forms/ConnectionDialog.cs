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

        // Blocking moving the caret with arrow keys
        textBox.KeyDown += (_, e) =>
        {
            if (textBox.Text == text && e.KeyCode is Keys.Left or Keys.Right or Keys.Up or Keys.Down)
            {
                e.Handled = true;
                textBox.SelectionStart = 0;
                textBox.SelectionLength = 0;
                return;
            }
            
            if (textBox.Text == text)
            {
                textBox.Text = "";
                textBox.ForeColor = Color.Black;
            }
        };

        // Blocking moving the caret with mouse
        textBox.MouseDown += (_, e) =>
        {
            if (textBox.Text == text && e.Button == MouseButtons.Left)
            {
                textBox.SelectionStart = 0;
                textBox.SelectionLength = 0;
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