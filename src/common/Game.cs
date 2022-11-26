using OkoCommon.Interface;

namespace OkoCommon;

public class GameTable
{
    private PlayerBase banker;
    private readonly List<PlayerBase> players;
    private PlayerBase? bankBrokePlayer;
    
    private IEnumerable<PlayerBase> AllPlayers => players.Append(banker);
    private IEnumerable<PlayerBase> AllExcept(PlayerBase player) => AllPlayers.Where(p => p != player);

    private void NotifyAllPlayers<T>(INotification<T> notification)
    {
        foreach (var player in AllPlayers)
        {
            player.Notify(notification);
        }
    }
    private void NotifyAllExcept<T>(PlayerBase except, INotification<T> notification)
    {
        foreach (var player in AllExcept(except))
        {
            player.Notify(notification);
        }
    }

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
        if (bankBrokePlayer is not null)
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
            
            NotifyAllPlayers(new GenericNotif<PlayerBase>(NotifEnum.NewBanker, banker));
        }
    }
    
    private void AssignBanker(PlayerBase newBanker)
    {
        banker = newBanker;
        players.Remove(banker);

        banker.Notify(new NoDataNotif(NotifEnum.SetInitialBank));
        
        initialBank = 100;
        bank = initialBank;
    }

    private void OneRound()
    {
        deck.Restart();
        
        var malaDomu = false;
        if (initialBank * 2 >= bank)
        {
            banker.Notify(new NoDataNotif(NotifEnum.AskForMalaDomu));
            malaDomu = banker.GetResponse<bool>().Data;

            if (malaDomu) { NotifyAllPlayers(new NoDataNotif(NotifEnum.MalaDomuCalled)); }
        }

        deck.Shuffle();
        
        banker.Notify(new NoDataNotif(NotifEnum.ChooseCutPlayer));
        var cutPlayer = banker.GetResponse<PlayerBase>().Data;

        var duelInitiated = CutAndDuel(cutPlayer!); // Should this warning be supressed? This should be null right?
        
        if (duelInitiated)
        {
            return;
        }
        
        foreach (var player in players.Where(player => player.Balance != 0).Append(banker))
        {
            player.Hand.Clear();
            player.Hand.Add(deck.Draw());
            player.Exchanged = false;
            
            player.Notify(new CardNotif(NotifEnum.ReceivedCard, player.Hand[0]));
        }
        
        if (malaDomu && banker.Hand[0].Rank is Rank.King or Rank.Eight)
        {
            NotifyAllPlayers(new NoDataNotif(NotifEnum.MalaDomuSuccess));
            
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
        cutPlayer.Notify(new NoDataNotif(NotifEnum.ChooseCutPosition));
        var cutIndex = cutPlayer.GetResponse<int>().Data;
        
        var cutCard = deck.Cut(cutIndex);
        NotifyAllPlayers(new GenericNotif<Card>(NotifEnum.SeeCutCard,cutCard));

        duelPlayer.Notify(new NoDataNotif(NotifEnum.DuelOffer));
        var bet = duelPlayer.GetResponse<int>().Data;
        
        if (bet == 0)
        {
            return false;
        }
        
        banker.Notify(new GenericNotif<int>(NotifEnum.DuelOffer, bet));
        var accept = banker.GetResponse<bool>().Data;
        
        if (!accept)
        {
            duelPlayer.Notify(new NoDataNotif(NotifEnum.DuelDeclined));
            return false;
        }
        
        duelPlayer.Notify(new NoDataNotif(NotifEnum.DuelAccepted));
        Duel(duelPlayer);

        return true;
    }

    private void Duel(PlayerBase duelPlayer)
    {
        duelPlayer.Notify(new NoDataNotif(NotifEnum.DuelAskNextCard));
        // ...
        
        banker.Notify(new NoDataNotif(NotifEnum.DuelAskNextCard));
        // ...
        
        // Announce winner
        
    }

    private void PlayersTurn(PlayerBase player)
    {
        
        while (true)
        {
            player.Notify(new NoDataNotif(NotifEnum.AskForCardDecision));
            // var decision = player.GetResponse<CardDecision>().Data;
            
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
            player.Notify(new NoDataNotif(NotifEnum.AlreadyExchanged));
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