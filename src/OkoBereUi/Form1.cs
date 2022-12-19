using OkoCommon.Game;

namespace OkoBereUi
{
    public partial class Form1 : Form
    {
        private List<PictureBox> cardBoxes;
        private Label balanceLabel;
        private Label betLabel;
        private Label bankLabel;
        private Label playerTurnLabel;
        private Button drawButton;
        private Button betButton;
        private Button endTurnButton;

        public Form1()
        {
            InitializeComponent();

            // Initialize card boxes
            cardBoxes = new List<PictureBox>();
            for (int i = 0; i < 5; i++)
            {
                PictureBox cardBox = new PictureBox();
                cardBox.Size = new Size(75, 100);
                cardBox.Location = new Point(10 + (80 * i), 10);
                cardBox.SizeMode = PictureBoxSizeMode.StretchImage;
                Controls.Add(cardBox);
                cardBoxes.Add(cardBox);
                
                cardBoxes[i].Image = ImageGetter.GetImage(new Card(Suit.Clubs, Rank.Ace));
            }

            // Initialize balance label
            balanceLabel = new Label();
            balanceLabel.AutoSize = true;
            balanceLabel.Location = new Point(10, 120);
            balanceLabel.Text = "Balance: ";
            Controls.Add(balanceLabel);

            // Initialize bet label
            betLabel = new Label();
            betLabel.AutoSize = true;
            betLabel.Location = new Point(10, 140);
            betLabel.Text = "Bet: ";
            Controls.Add(betLabel);

            // Initialize bank label
            bankLabel = new Label();
            bankLabel.AutoSize = true;
            bankLabel.Location = new Point(10, 160);
            bankLabel.Text = "Bank: ";
            Controls.Add(bankLabel);

            // Initialize player turn label
            playerTurnLabel = new Label();
            playerTurnLabel.AutoSize = true;
            playerTurnLabel.Location = new Point(10, 180);
            playerTurnLabel.Text = "Player's Turn";
            Controls.Add(playerTurnLabel);

            // Initialize draw button
            drawButton = new Button();
            drawButton.Size = new Size(75, 23);
            drawButton.Location = new Point(10, 210);
            drawButton.Text = "Draw";
            drawButton.Click += new EventHandler(DrawButton_Click!);
            Controls.Add(drawButton);

            // Initialize bet button
            betButton = new Button();
            betButton.Size = new Size(75, 23);
            betButton.Location = new Point(95, 210);
            betButton.Text = "Bet";
            betButton.Click += new EventHandler(BetButton_Click!);
            Controls.Add(betButton);

            // Initialize end turn button
            endTurnButton = new Button();
            endTurnButton.Size = new Size(75, 23);
            endTurnButton.Location = new Point(180, 210);
            endTurnButton.Text = "End Turn";
            endTurnButton.Click += new EventHandler(EndTurnButton_Click!);
            Controls.Add(endTurnButton);
        }

        private void DrawButton_Click(object sender, EventArgs e)
        {
            // Send request to server
            
            // Receive response from server
        }

        private void BetButton_Click(object sender, EventArgs e)
        {
            // Send request to server
            
            // Receive response from server
        }

        private void EndTurnButton_Click(object sender, EventArgs e)
        {
            // End the player's turn and start the next player's turn
            EndTurn();
            StartNextTurn();
        }
        
        private void PlaceBet(int amount)
        {
            // Place a bet of the specified amount
            betLabel.Text = $"Bet: ${amount}";
        }

        private void EndTurn()
        {
            // End the current player's turn
            playerTurnLabel.Text = "";
        }

        private void StartNextTurn()
        {
            // Start the next player's turn
            playerTurnLabel.Text = "Next Player's Turn";
        }
    }
}