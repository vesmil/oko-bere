using System.Diagnostics;
using OkoCommon.Communication;

namespace OkoCommon.Game;

public interface IGame
{
    public void Lobby();
    public void GameLoop();
    public void OnNewPlayer(PlayerBase player);
}

public partial class Game : IGame
{
    private readonly Deck deck = new();

    private readonly GetPlayersDelegate getGetPlayersDelegate;
    private readonly GameTable table;

    public Game(GetPlayersDelegate getGetPlayerDel)
    {
        getGetPlayersDelegate = getGetPlayerDel;
        table = new GameTable(getGetPlayersDelegate.Invoke());
        
        table.NotifyAllPlayers(Notification.Create(NotifEnum.UpdateGameState, CreateGameState()));
    }

    public void Lobby()
    {
        while (true)
        {
            table.UpdatePlayers(getGetPlayersDelegate.Invoke());

            if (table.AllPlayers.Count > 2)
            {
                // Might also add timeout or waiting for confirmation from all players
                table.AskForContinue();
                break;
            }

            Thread.Sleep(200);
        }
    }

    public void GameLoop()
    {
        table.UpdatePlayers(getGetPlayersDelegate.Invoke());
        Debug.WriteLine($"Starting game loop with {table.AllPlayers.Count} players");
        
        while (true)
        {
            table.SetBanker();
            
            while (table.Bank > 0)
            {
                var newPlayers = getGetPlayersDelegate.Invoke();
                if (newPlayers.Count != table.AllPlayers.Count) table.UpdatePlayers(newPlayers);

                OneRound();
            }

            if (!table.AskForContinue()) break;
        }
        
        table.NotifyAllPlayers(Notification.Create(NotifEnum.EndOfGame));
    }

    private readonly Mutex addingMutex = new();
    public void OnNewPlayer(PlayerBase newPlayer)
    {
        addingMutex.WaitOne();  // allows only one thread to add a player at a time
        
        table.NotifyAllPlayers(Notification.Create(NotifEnum.NewPlayer, newPlayer.ToPlayerInfo()));
        
        table.AllPlayers.Add(newPlayer);

        var gameState = CreateGameState();
        newPlayer.Notify(Notification.Create(NotifEnum.UpdateGameState, gameState));
    }

    public static Game PlayAgainstComputer(int numPlayers)
    {
        // TODO I will change it to multiple players on the same computer
        return new Game(() => new List<PlayerBase> { new AiPlayer("Player", 1000), new AiPlayer("Computer", 1000) });
    }

    private void OneRound()
    {
        if (table.Banker is null) throw new Exception("Can not start round without a banker.");

        deck.Restart();

        var malaDomu = false;

        if (table.InitialBank * 2 <= table.Bank)
        {
            if (table.Banker is not null)
            {
                table.Banker.Notify(Notification.Create(NotifEnum.AskMalaDomu));
                malaDomu = table.Banker.GetResponse<bool>().Data;
            }
            else
                throw new Exception("Banker is missing");

            if (malaDomu)
            {
                Debug.WriteLine("Mala domu was called");
                table.NotifyPlayers(Notification.Create(NotifEnum.MalaDomuCalled));
            }
        }

        deck.Shuffle();
        Debug.WriteLine("Deck was shuffled");

        PlayerBase? cutPlayer = null;
        while (cutPlayer is null)
        {
            table.Banker.Notify(Notification.Create(NotifEnum.AskChooseCutPlayer));
            var cutId = table.Banker.GetResponse<Guid>().Data;
            
            cutPlayer = table.Players.FirstOrDefault(x => x.Id == cutId);
        }

        var duelInitiated = CutAndDuel(cutPlayer);
        if (duelInitiated) return;

        foreach (var player in table.AllPlayers.Where(p => p.Balance != 0).Append(table.Banker))
        {
            player.Hand.Clear();
            player.Hand.Add(deck.Draw());
            player.Exchanged = false;

            player.Notify(Notification.Create(NotifEnum.ReceivedCard, player.Hand[0]));
        }

        if (malaDomu && table.Banker.Hand[0].Rank is Rank.King or Rank.Eight)
        {
            table.NotifyAllPlayers(Notification.Create(NotifEnum.MalaDomuSuccess));

            table.Banker.Balance += table.Bank;
            table.Bank = 0;

            return;
        }

        foreach (var player in table.AllPlayers) PlayersTurn(player);
        BankersTurn();
        Evaluation();
    }

    private bool CutAndDuel(PlayerBase cutPlayer)
    {
        if (table.Banker is null) throw new Exception("Can not start duel without a banker.");

        var index = table.AllPlayers.IndexOf(cutPlayer);
        var duelPlayer = table.AllPlayers[(index + 1) % table.AllPlayers.Count];

        Debug.WriteLine($"Cutting by {cutPlayer.Name} and duel by {duelPlayer.Name}");
        cutPlayer.Notify(Notification.Create(NotifEnum.AskChooseCutPosition));
        var cutIndex = cutPlayer.GetResponse<int>().Data;

        var cutCard = deck.Cut(cutIndex);
        table.NotifyAllPlayers(Notification.Create(NotifEnum.ShowCutCard, cutCard));

        duelPlayer.Notify(Notification.Create(NotifEnum.AskDuel));
        var bet = duelPlayer.GetResponse<int>().Data;

        if (bet == 0) return false;

        table.Banker.Notify(Notification.Create(NotifEnum.AskDuel, bet));
        var accept = table.Banker.GetResponse<bool>().Data;

        if (!accept)
        {
            duelPlayer.Notify(Notification.Create(NotifEnum.DuelDeclined));
            return false;
        }

        duelPlayer.Notify(Notification.Create(NotifEnum.DuelAccepted));
        Duel(duelPlayer);

        return true;
    }

    private void Duel(PlayerBase duelPlayer)
    {
        // TODO complete
        duelPlayer.Notify(Notification.Create(NotifEnum.AskTurnNoBet));
        table.Banker!.Notify(Notification.Create(NotifEnum.AskTurnNoBet));
        
    }

    private void PlayersTurn(PlayerBase player, bool noBet = false)
    {
        while (true)
        {
            player.Notify(noBet
                ? Notification.Create(NotifEnum.AskTurnNoBet)
                : Notification.Create(NotifEnum.AskTurn));

            player.Notify(Notification.Create(NotifEnum.AskTurn));
            var decision = player.GetResponse<TurnDecision>().Data;

            switch (decision)
            {
                case TurnDecision.Bet:
                    if (!Bet(player, noBet)) return;
                    DrawCard(player);
                    break;
                
                case TurnDecision.Draw:
                    DrawCard(player);
                    break;
                
                case TurnDecision.Stop:
                    return;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (player.Hand.IsInstantWin())
            {
                player.Notify(Notification.Create(NotifEnum.Won));
                // TODO notify others
                return;
            }
            
            if (player.Hand.IsBust())
            {
                player.Notify(Notification.Create(NotifEnum.Bust));
                return;
            }
            
            if (player.Hand.IsExchangeable()) ExchangeCards(player);
        }
    }

    private static bool Bet(PlayerBase player, bool noBet = false)
    {
        if (noBet)
        {
            // throw new Exception("Can not bet in duel.");
            return false;
        }
        
        var bet = player.GetResponse<int>().Data;
        if (bet > player.Balance)
        {
            // throw new Exception("Can not bet more than you have.");
            return false;
        }
        
        player.Bet = bet;
        player.Balance -= bet;

        return true;
    }

    private void BankersTurn()
    {
        while (true)
        {
            var card = deck.Draw();
            table.Banker!.Hand.Add(card);

            table.Banker.Notify(Notification.Create(NotifEnum.ReceivedCard, card));
            table.NotifyAllExcept(table.Banker, Notification.Create(NotifEnum.OtherReceivesCard, table.Banker));

            if (table.Banker.Hand.IsBust()) return;
            if (table.Banker.Hand.IsExchangeable()) ExchangeCards(table.Banker);
            
            table.Banker.Notify(Notification.Create(NotifEnum.AskTurnNoBet));
            var decision = table.Banker.GetResponse<TurnDecision>().Data;
            
            if (decision == TurnDecision.Stop) return;
        }
    }

    private void DrawCard(PlayerBase player)
    {
        var card = deck.Draw();
        player.Hand.Add(card);

        player.Notify(Notification.Create(NotifEnum.ReceivedCard, card));
        table.NotifyAllExcept(player, Notification.Create(NotifEnum.OtherReceivesCard, player));
    }

    private void ExchangeCards(PlayerBase player)
    {
        player.Notify(Notification.Create(NotifEnum.AskExchange));
        if (!player.GetResponse<bool>().Data) return;
        
        player.Hand.Clear();

        var newCard = deck.Draw();
        player.Hand.Add(newCard);

        player.Notify(Notification.Create(NotifEnum.ReceivedCard, newCard));
        table.NotifyAllExcept(player, Notification.Create(NotifEnum.OtherExchanged, player));
        
    }

    private void Evaluation()
    {
        foreach (var player in table.AllPlayers.Where(player => 
                     player.Hand.GetBestValue() > table.Banker!.Hand.GetBestValue()))
        {
            player.Balance += 2 * player.Bet;
            player.Bet = 0;
            player.Notify(Notification.Create(NotifEnum.Won));
        }

        table.Bank += table.AllPlayers.Sum(player => player.Bet);
        table.AllPlayers.ForEach(player => player.Bet = 0);
        
        // NOTE might be better to notify players about their balance change
        table.NotifyAllPlayers(Notification.Create(NotifEnum.UpdateGameState, CreateGameState()));
    }

    private GameState CreateGameState()
    {
        return new GameState(table.AllPlayers, table.Bank);
    }
}