using System.Net.Sockets;
using OkoCommon;

namespace OkoServer;

public class OkoBere
{
    private TcpPlayer banker;
    private readonly List<TcpPlayer> players;
    
    private readonly List<(TcpPlayer, int)> currentBets = new();

    private int bank;
    private int initialBank;

    private bool endPending;
    
    private readonly Deck deck = new();

    public OkoBere(List<TcpPlayer> players)
    {
        banker = players[0];
        this.players = players;
    }

    public void Start()
    {
        banker = players[0];

        var welcomeString = "None\n\n" + "Welcome to the game!\n" +
                            "The banker is " + banker.Name + " and has " + banker.Balance + " money.\n" +
                            "The players are:\n" + 
                            players.Aggregate("", (current, player) => current + "Player " + player.Name + " has " + player.Balance + " money.\n");

        foreach (var player in players)
        {
            player.SendString(welcomeString);
        }

        banker.SendString("You\n\n" + "Welcome to the game!\n" +
                          "You are the banker and have " + banker.Balance + " money.\n" +
                          "The players are:\n" +
                          players.Aggregate("", (current, player) =>
                                  current + "Player " + player.Name + " has " + player.Balance + " money.\n") + 
                          "\nSo, how much money do you want to put in the bank?");

        var bankerInput = banker.ReceiveString();
        
        while (int.TryParse(bankerInput, out bank) == false || banker.Balance < bank) {
            banker.SendString("You have " + banker.Balance + " money. Please enter a valid amount.");
            bankerInput = banker.ReceiveString();
        }

        banker.Balance -= bank;
        initialBank = bank;
        
        while (bank > 0) OneRound();

        // ...
    }
    
    private void OneRound()
    {
        var message = "Banker\n\n" + "Round started!\n" +
                      "There is currently " + bank + " money in the bank.\n";
        
        foreach (var player in players)
        {
            player.SendString(message);
        }
        
        message = "You\n\n" + "Round started!\n" +
                  "There is currently " + bank + " money in the bank.\n" +
                  "Banker, would you like to end this game? (y/n)";
        
        banker.SendString(message);
        endPending = banker.ReceiveString() == "y";
        
        if (endPending && bank < initialBank * 2)
        {
            banker.SendString("Sorry, you can't yet. The initial bank was " + initialBank + ", so you need to have at least " + initialBank * 2 + " to end the game.");
            endPending = false;
        }
        
        deck.Shuffle();
        
        foreach (var player in players)
        {
            player.Hand.Clear();
            banker.Hand.Clear();

            player.Hand.Add(deck.Draw());
            banker.Hand.Add(deck.Draw());

            player.Exchanged = false;
            banker.Exchanged = false;
        }

        Console.WriteLine("The cards have been dealt.\n");

        if (endPending)
        {
            if (banker.Hand[0].Rank is Rank.King or Rank.Eight)
            {
                Console.WriteLine("The banker has a " + banker.Hand[0].Rank + " of " + banker.Hand[0].Suit + ".\n");
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

    private void PlayersTurn(TcpPlayer player)
    {
        Console.WriteLine($"{player.Name}'s turn.");
        Console.WriteLine($"{player.Name} has {player.Balance} in their account.");
        Console.WriteLine($"{player.Name}, your first card is {player.Hand[0]}.");
        Console.WriteLine("The bank is " + bank);

        // if (player.OptionToChange()) ExchangeCards(player);

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
            
            // if (player.OptionToChange()) ExchangeCards(player);

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

    private void DrawCard(TcpPlayer player)
    {
        var card = deck.Draw();
        player.Hand.Add(card);
        
        player.SendString("You\n\n" + "You have drawn a " + card + "\n" +
                          "Your total count is " + player.Total() + ".\n");
                        
        // if (player.OptionToChange()) ExchangeCards(player);
    }

    private void ExchangeCards(TcpPlayer player)
    {
        if (player.Exchanged) return;
        
        Console.WriteLine("Would you like to change your bet? (y/n)");
            
        if (Console.ReadLine() == "y")
        {
            player.Hand.Clear();
            var card = deck.Draw();
            player.Hand.Add(card);
            Console.WriteLine($"{player.Name} has drawn a {card}.");
            
            player.Exchanged = true;
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