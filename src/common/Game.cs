using OkoCommon.Interface;

namespace OkoCommon;

public class GameTable
{
    private PlayerBase banker;
    private readonly List<PlayerBase> players;
    private PlayerBase? bankBrokePlayer;
    
    private IEnumerable<PlayerBase> AllPlayers => players.Append(banker);
    private IEnumerable<PlayerBase> AllExcept(PlayerBase player) => AllPlayers.Where(p => !Equals(p, player));

    /*
    private void NotifyAllPlayers(INotification notification)
    {
        foreach (var player in AllPlayers)
        {
            player.SendNotification(notification);
        }
    }
    private void NotifyAllExcept(PlayerBase except, INotification notification)
    {
        foreach (var player in AllExcept(except))
        {
            player.SendNotification(notification);
        }
            
    }
    */

    private readonly Dictionary<PlayerBase, int> currentBets;

    private int bank;
    private int initialBank = 0;
    
    private readonly Deck deck = new();

    public GameTable(List<PlayerBase> players)
    {
        banker = players[0];
        this.players = players;

        currentBets = new Dictionary<PlayerBase, int>();
        ClearBets();
    }

    public void GameLoop()
    {
        while (true)
        {
            SetBanker();

            while (bank > 0) { OneRound(); }

            // TODO decide if game should continue
            
            // Notify
            // GetResponse
            // If count of players is...
            
            if( new Random().Next(2) == 1)
            {
                break;
            }
        }
    }

    private void SetBanker()
    {
        // Banker is either the one who took bank or raffled
        if (bankBrokePlayer != null)
        {
            AssignBanker(bankBrokePlayer);
            bankBrokePlayer = null;
        }
        else
        {
            // Raffle includes even the original banker
            if (!players.Contains(banker))
            {
                players.Add(banker);
            }
        
            var num = new Random().Next(players.Count);

            // Might add animation for the raffle here
            AssignBanker(players[num]);
        }
    }
    
    private void AssignBanker(PlayerBase newBanker)
    {
        banker = newBanker;
        players.Remove(banker);

        // TODO
        
        // NotifyAll();
        // let the banker choose the initial bank
        // banker.SendAction();
        // ...
        
        initialBank = 100;
        bank = initialBank;
    }

    private void OneRound()
    {
        deck.Restart();
        
        
        var malaDomu = false;
        if (initialBank * 2 >= bank)
        {
            // TODO
            // Ask about "Malá domů"

            banker.SendNotification(null!);
            malaDomu = true;
        }

        deck.Shuffle();
        
        // Ask banker who should cut
        // ...
        
        var duelInitiated = CutAndDuel(players[0]);
        
        if (duelInitiated)
        {
            return;
        }
        
        foreach (var player in players.Where(player => player.Balance != 0).Append(banker))
        {
            player.Hand.Clear();
            player.Hand.Add(deck.Draw());
            player.Exchanged = false;
        }

        // Cards have been dealt - show them to the players
        
        if (malaDomu && banker.Hand[0].Rank is Rank.King or Rank.Eight)
        {
            // Notify about banker's hand and end the round
            banker.Balance += bank;
            bank = 0;
            
            return;
        }

        foreach (var player in players)
        {
            PlayersTurn(player);
        }

        BankersTurn();
        
        Evaluation();
    }

    private void ClearBets()
    {
        foreach (var player in players)
        {
            currentBets[player] = 0;
        }
    }

    private bool CutAndDuel(PlayerBase cutPlayer)
    {
        var index = players.IndexOf(cutPlayer);
        var duelPlayer = players[(index + 1) % players.Count];
        
        // Let the cutPlayer choose where to cut
        const int cutIndex = 0;
        deck.Cut(cutIndex);
        
        // Start duel
        // Show the visible card...
        // Ask both players if they want to play


        return true;
    }

    private void PlayersTurn(PlayerBase player)
    {
        while (true)
        {
            // Ask player for decision
            // Send Would you like to next, bet, exchane or end
            
            // if(...)
            {
                
            }

            DrawCard(player);
            
            if (player.Hand.IsBust())
            {
                break;
            }
        }
    }
    
    private void BankersTurn()
    {
        var card = deck.Draw();
        banker.Hand.Add(card);
        
        // Ask if he would like to make it visible
        
        // ...
    }

    private void DrawCard(PlayerBase player)
    {
        var card = deck.Draw();
        player.Hand.Add(card);
        
        // player.SendNotification(... , card);
        
        foreach (var otherPlayer in AllExcept(player))
        {
            // otherPlayer.SendNotification(...);
        }
    }

    private void ExchangeCards(PlayerBase player)
    {
        if (player.Exchanged)
        {
            // Notify player that he has already exchanged
            // ...
        }
        
        player.Hand.Clear();
        player.Hand.Add(deck.Draw());
        
        // Notify player about his new hand
        // ...
    }
    
    private void Evaluation()
    {
        // Inform the players...
        
        foreach (var player in players.Where(player => player.Hand.GetBestValue() > banker.Hand.GetBestValue()))
        {
            player.Balance += 2 * currentBets[player];
            currentBets[player] = 0;
        }
        
        bank += currentBets.Values.Sum();
    }
}