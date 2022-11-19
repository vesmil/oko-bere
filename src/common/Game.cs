namespace OkoCommon;

public class OkoBere
{
    private Player banker;
    private readonly List<Player> players;

    private readonly List<(Player, int)> currentBets = new();

    private int bank;
    private int initialBank;

    private bool endPending;
    
    private readonly Deck deck = new();

    public OkoBere()
    {
        banker = new Player("Banker", 100);
        players = new List<Player>
        {
            new("Player 1", 100),
            new("Player 2", 100),
            new("Player 3", 100)
        };
    }

    public void Start()
    {
        Console.WriteLine("Welcome to the game!\n");
        Console.WriteLine("The banker is " + banker.Name + " and has " + banker.Balance + " money.");
        
        foreach (var player in players)
            Console.WriteLine("Player " + player.Name + " has " + player.Balance + " money.");

        Console.WriteLine("------------------------------------------------------");
        Console.WriteLine("So banker, how much would you like to put into the bank?");

        while (int.TryParse(Console.ReadLine(), out bank) == false || banker.Balance < bank)
            Console.WriteLine("Please enter a valid amount.");

        Console.WriteLine();
        
        banker.Balance -= bank;
        initialBank = bank;

        Console.WriteLine($"The bank has {bank} in it.");
        Console.WriteLine("Let's play!\n\n");

        while (bank > 0) OneRound();

        Console.WriteLine("Would anyone like to play again? (y/n)");
        Console.WriteLine("Let's decide the banker");
        
        Console.Read();
    }
    
    private void OneRound()
    {
        Console.WriteLine("Round started!\n");
        Console.WriteLine("Banker, would you like to end this game? (y/n)");
        
        endPending = Console.ReadLine() == "y";
        
        if (endPending && bank < initialBank * 2)
        {
            Console.WriteLine("Sorry, you can't yet. The initial bank was " + initialBank + ", so you need to have at least " + initialBank * 2 + " to end the game.");
            endPending = false;
        }
        
        deck.Shuffle();
        
        foreach (var player in players)
        {
            player.Hand.Clear();
            banker.Hand.Clear();

            player.Hand.Add(deck.Draw());
            banker.Hand.Add(deck.Draw());

            player.Exchnaged = false;
            banker.Exchnaged = false;
        }

        Console.WriteLine("The cards have been dealt.\n");

        if (endPending)
        {
            if (banker.Hand[0].rank is Rank.King or Rank.Eight)
            {
                Console.WriteLine("The banker has a " + banker.Hand[0].rank + " of " + banker.Hand[0].suit + ".\n");
                Console.WriteLine("So he gets to keep the bank.");
                banker.Balance += bank;
                bank = 0;
                
                return;
            }
        }
        
        foreach (var player in players) PlayersTurn(player);
        
        DealersTurn();
        Evaluation();
    }

    private void PlayersTurn(Player player)
    {
        Console.WriteLine($"{player.Name}'s turn.");
        Console.WriteLine($"{player.Name} has {player.Balance} in their account.");
        Console.WriteLine($"{player.Name}, your first card is {player.Hand[0]}.");
        Console.WriteLine("The bank is " + bank);

        if (player.OptionToChange()) ExchangeCards(player);

        if (player.Balance > 0)
        {
            Console.WriteLine("How much would you like to bet?");

            int bet;
            
            while (int.TryParse(Console.ReadLine(), out bet) == false || bet > player.Balance || bet < 0)
                Console.WriteLine("Please enter a valid amount.");

            player.Balance -= bet;
            bank -= bet;

            currentBets.Add((player, bet));

            Console.WriteLine($"{player.Name} has bet {bet}.");

            var card = deck.Draw();
            player.Hand.Add(card);

            if (player.InstantWin())
            {
                Console.WriteLine($"{player.Name} has won {bet}!");
                player.Balance += bet * 2;
                currentBets.Remove((player, bet));
                return;
            }
            
            Console.WriteLine("You have drawn a " + card);
            Console.WriteLine($"Your total count is {player.Total()}.");
            
            if (player.OptionToChange()) ExchangeCards(player);

            while (player.Total() < 21)
            {
                Console.WriteLine("Would you like to raise your bet? (y/n)");
                
                if (Console.ReadLine() == "y")
                {
                    Console.WriteLine("How much would you like to raise?");

                    int raise;
                    
                    while (int.TryParse(Console.ReadLine(), out raise) == false || raise > player.Balance || raise < 0)
                        Console.WriteLine("Please enter a valid amount.");

                    player.Balance -= raise;
                    bank -= raise;

                    var betIndex = currentBets.FindIndex(x => x.Item1 == player);
                    currentBets[betIndex] = (player, currentBets[betIndex].Item2 + raise);
                    
                    Console.WriteLine($"{player.Name} has bet {bet + raise}.");

                    DrawCard(player);
                }
                else
                {
                    Console.WriteLine("Would you like nonetheless draw another card? (y/n)");

                    if (Console.ReadLine()?.ToLower() == "y")
                        DrawCard(player);
                    
                    else
                        break;
                }
            }

            if (player.Total() > 21)
            {
                Console.WriteLine("You have busted!\n");
                
                bank += 2 * currentBets.FindAll(x => x.Item1 == player).Sum(x => x.Item2);
                currentBets.RemoveAll(x => x.Item1 == player);
                
                player.Hand.Clear();
            }
            
            Console.WriteLine();
        }
        
        else Console.WriteLine($"{player.Name} has no money left to bet.");
    }

    private void DrawCard(Player player)
    {
        var card = deck.Draw();
        player.Hand.Add(card);
        Console.WriteLine("You have drawn a " + card);
        Console.WriteLine($"Your total count is {player.Total()}.");
                        
        if (player.OptionToChange()) ExchangeCards(player);
    }

    private void ExchangeCards(Player player)
    {
        if (player.Exchnaged) return;
        
        Console.WriteLine("Would you like to change your bet? (y/n)");
            
        if (Console.ReadLine() == "y")
        {
            player.Hand.Clear();
            var card = deck.Draw();
            player.Hand.Add(card);
            Console.WriteLine($"{player.Name} has drawn a {card}.");
            
            player.Exchnaged = true;
        }
    }

    private void DealersTurn()
    {
        Console.WriteLine("Now it's the dealer's turn.");
                
        while (banker.Total() < 21)
        {
            Console.WriteLine("The dealer has drawn a " + banker.Hand[0]);
                    
            Console.WriteLine("Would you like to draw another card? (y/n)");

            if (Console.ReadLine()?.ToLower() == "y")
                DrawCard(banker);
            else break;
        }
        
        Console.WriteLine("The banker's total count is " + banker.Total());
    }
    private void Evaluation()
    {
        if (banker.Total() > 21)
        {
            Console.WriteLine("The banker has busted!\n");

            foreach (var (player, bet) in currentBets)
            {
                player.Balance += 2 * bet;
            }
            
            currentBets.Clear();
        }
        
        else foreach (var (player, bet) in currentBets)
        {
            if (player.Total() > banker.Total())
            {
                Console.WriteLine($"{player.Name} has won {bet}.");
                player.Balance += bet * 2;
            }
            else
            {
                Console.WriteLine($"{player.Name} has lost {bet}.");
                bank += 2 * bet;
            }
        }
    }
}