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
        
        table.NotifyAllPlayers(Notification.Create(NotifEnum.GameStateInfo, CreateGameState()));
    }

    public void Lobby()
    {
        while (true)
        {
            // TODO handle players actions
            
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
        newPlayer.Notify(Notification.Create(NotifEnum.GameStateInfo, gameState));
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
                table.Banker.Notify(Notification.Create(NotifEnum.AskForMalaDomu));
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
        Console.WriteLine("Deck was shuffled");

        PlayerBase? cutPlayer = null;
        while (cutPlayer is null)
        {
            table.Banker.Notify(Notification.Create(NotifEnum.ChooseCutPlayer));
            var cutPlayerName = table.Banker.GetResponse<string>().Data;
            cutPlayer = table.Players.FirstOrDefault(x => x.Name == cutPlayerName);
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

        // Let the cutPlayer choose where to cut
        cutPlayer.Notify(Notification.Create(NotifEnum.ChooseCutPosition));
        var cutIndex = cutPlayer.GetResponse<int>().Data;

        var cutCard = deck.Cut(cutIndex);
        table.NotifyAllPlayers(Notification.Create(NotifEnum.ShowCutCard, cutCard));

        duelPlayer.Notify(Notification.Create(NotifEnum.DuelOffer));
        var bet = duelPlayer.GetResponse<int>().Data;

        if (bet == 0) return false;

        table.Banker.Notify(Notification.Create(NotifEnum.DuelOffer, bet));
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
        duelPlayer.Notify(Notification.Create(NotifEnum.AskForTurnNoBet));
        table.Banker!.Notify(Notification.Create(NotifEnum.AskForTurnNoBet));
        
    }

    private void PlayersTurn(PlayerBase player, bool noBet = false)
    {
        while (true)
        {
            player.Notify(noBet
                ? Notification.Create(NotifEnum.AskForTurnNoBet)
                : Notification.Create(NotifEnum.AskForTurn));

            player.Notify(Notification.Create(NotifEnum.AskForTurn));
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
            
            table.Banker.Notify(Notification.Create(NotifEnum.AskForTurnNoBet));
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
        player.Notify(Notification.Create(NotifEnum.AskForExchange));
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
        table.NotifyAllPlayers(Notification.Create(NotifEnum.GameStateInfo, CreateGameState()));
    }

    private GameState CreateGameState()
    {
        var gameState = new GameState();

        foreach (var player in table.AllPlayers)
            gameState.Players.Add(player.ToPlayerInfo());

        return gameState;
    }
}