using System.Diagnostics;
using OkoCommon.Communication;

namespace OkoCommon.Game;

/// <summary>
///     Interface to define what a game should be able to do.
///     This is used to allow extensibility of the server.
/// </summary>
public interface IGame
{
    public void Lobby();
    public void GameLoop();
    public void OnNewPlayer(PlayerBase player);
}


/// <summary>
///     Implementation of the Oko game.
/// </summary>
public partial class OkoGame : IGame
{
    private readonly Deck deck = new();
    private readonly GameTable table;
    
    // Delegate to get all player connected to the server - could also be used to separate different games
    private readonly GetPlayersDelegate getGetPlayersDelegate;

    // To ensure save adding of players
    private readonly Mutex addingMutex = new();

    public OkoGame(GetPlayersDelegate getGetPlayerDel)
    {
        getGetPlayersDelegate = getGetPlayerDel;
        table = new GameTable(getGetPlayersDelegate.Invoke());
        UpdateGameStateForAll();
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

    /// <summary>
    ///     Player was added to the game - notify all players and the new player
    /// </summary>
    /// <param name="newPlayer"></param>
    public void OnNewPlayer(PlayerBase newPlayer)
    {
        addingMutex.WaitOne(); // allows only one thread to add a player at a time

        table.NotifyAllPlayers(Notification.Create(NotifEnum.NewPlayer, newPlayer.ToPlayerInfo()));
        table.AllPlayers.Add(newPlayer);

        newPlayer.Notify(Notification.Create(NotifEnum.UpdateGameState, CreateGameState(newPlayer)));
    }

    private void OneRound()
    {
        if (table.Banker is null) throw new Exception("Can not start round without a banker.");

        deck.Restart();
        var malaDomu = false;

        UpdateGameStateForAll();

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

        foreach (var player in table.AllPlayers)
        {
            player.Hand.Clear();
            player.Exchanged = false;
            DrawCard(player);
        }

        if (malaDomu && table.Banker.Hand[0].Rank is Rank.King or Rank.Eight)
        {
            table.NotifyAllPlayers(Notification.Create(NotifEnum.MalaDomuSuccess));

            table.Banker.Balance += table.Bank;
            table.Bank = 0;

            return;
        }

        foreach (var player in table.AllPlayers.Where(p => !p.IsBanker)) PlayersTurn(player);
        BankersTurn();

        Evaluate();
    }

    /// <summary>
    ///     Ask a player to cut the deck and duel with the next player.
    /// </summary>
    /// <param name="cutPlayer">Player that cuts the deck</param>
    /// <returns>Whether a duel was initiated.</returns>
    private bool CutAndDuel(PlayerBase cutPlayer)
    {
        if (table.Banker is null) throw new Exception("Can not start duel without a banker.");

        var index = table.AllPlayers.IndexOf(cutPlayer);
        var duelPlayer = table.AllPlayers[(index + 1) % table.AllPlayers.Count];
        if (duelPlayer == table.Banker) duelPlayer = table.AllPlayers[(index + 2) % table.AllPlayers.Count];

        Debug.WriteLine($"Cutting by {cutPlayer.Name} and duel by {duelPlayer.Name}");
        
        cutPlayer.Notify(Notification.Create(NotifEnum.AskChooseCutPosition));
        var cutIndex = cutPlayer.GetResponse<int>().Data;

        var cutCard = deck.Cut(cutIndex);
        table.NotifyAllPlayers(Notification.Create(NotifEnum.ShowCutCard, cutCard));

        duelPlayer.Notify(Notification.Create(NotifEnum.AskDuel));
        var bet = duelPlayer.GetResponse<int>().Data;

        if (bet == 0) return false;

        table.Bank -= bet;
        duelPlayer.Balance -= bet;
        duelPlayer.Bet = bet;

        duelPlayer.Hand.Add(cutCard);
        duelPlayer.Notify(Notification.Create(NotifEnum.ReceivedCard, cutCard));
        table.NotifyAllExcept(duelPlayer, Notification.Create(NotifEnum.OtherReceivesCard, duelPlayer.Id));
        
        table.Banker.Hand.Add(cutCard);
        table.Banker.Notify(Notification.Create(NotifEnum.ReceivedCard, cutCard));
        table.NotifyAllExcept(table.Banker, Notification.Create(NotifEnum.OtherReceivesCard, table.Banker.Id));
        
        table.NotifyAllExcept(duelPlayer, Notification.Create(NotifEnum.OtherDuel, duelPlayer.Name));
        
        UpdateGameStateForAll();

        Duel(duelPlayer);
        return true;
    }

    /// <summary>
    ///     Entire duel between <paramref name="duelPlayer" /> and the banker
    /// </summary>
    private void Duel(PlayerBase duelPlayer)
    {
        PlayersTurn(duelPlayer, true);

        if (!duelPlayer.Hand.IsInstantWin() && !duelPlayer.Hand.IsBust())
        {
            PlayersTurn(table.Banker!, true);

            var won = table.Banker!.Hand.GetBestValue() >= duelPlayer.Hand.GetBestValue() &&
                      !table.Banker.Hand.IsBust();
            
            duelPlayer.Notify(Notification.Create(won ? NotifEnum.Won : NotifEnum.Lost, duelPlayer.Name));
            table.NotifyAllExcept(duelPlayer, Notification.Create(won ? NotifEnum.OtherLost : NotifEnum.OtherWins, duelPlayer.Name));
        }

        Evaluate();
    }

    /// <summary>
    ///     Player's turn - ask for decision and execute it until player stops or busts
    /// </summary>
    private void PlayersTurn(PlayerBase player, bool noBet = false)
    {
        while (true)
        {
            player.Notify(noBet
                ? Notification.Create(NotifEnum.AskTurnNoBet)
                : Notification.Create(NotifEnum.AskTurn));

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
            }

            if (player.Hand.IsInstantWin())
            {
                player.Notify(Notification.Create(NotifEnum.Won));
                table.NotifyAllExcept(player, Notification.Create(NotifEnum.OtherWins, player.Name));
                return;
            }

            if (player.Hand.IsBust())
            {
                player.Notify(Notification.Create(NotifEnum.Bust));
                table.NotifyAllExcept(player, Notification.Create(NotifEnum.OtherBusts, player.Name));
                return;
            }

            TryExchangeCards(player);
        }
    }

    /// <summary>
    ///     Draw a card for the player and notify all players.
    /// </summary>
    private bool Bet(PlayerBase player, bool noBet = false)
    {
        if (noBet)
        {
            return false;
        }

        var bet = player.GetResponse<int>().Data;
        if (bet > player.Balance)
        {
            return false;
        }

        player.Bet = bet;
        player.Balance -= bet;
        table.Bank -= bet;
        
        player.Notify(Notification.Create(NotifEnum.UpdateGameState, CreateGameState(player)));

        return true;
    }

    /// <summary>
    ///     The entire banker's turn - draw cards until he stops or busts.
    /// </summary>
    /// <exception cref="Exception"></exception>
    private void BankersTurn()
    {
        if (table.Banker is null) throw new Exception("Can not start round without a banker.");

        while (true)
        {
            table.Banker.Notify(Notification.Create(NotifEnum.AskTurnNoBet));
            var decision = table.Banker.GetResponse<TurnDecision>().Data;

            if (decision == TurnDecision.Stop) return;

            var card = deck.Draw();
            table.Banker!.Hand.Add(card);

            table.Banker.Notify(Notification.Create(NotifEnum.ReceivedCard, card));
            table.NotifyAllExcept(table.Banker, Notification.Create(NotifEnum.OtherReceivesCard, table.Banker.Id));

            if (table.Banker.Hand.IsBust()) return;
            TryExchangeCards(table.Banker);
        }
    }

    /// <summary>
    ///     Draws a card from the deck and adds it to the player's hand while notifying the player and the table.
    /// </summary>
    /// <param name="player"></param>
    private void DrawCard(PlayerBase player)
    {
        var card = deck.Draw();
        player.Hand.Add(card);
        player.Notify(Notification.Create(NotifEnum.ReceivedCard, card));
        table.NotifyAllExcept(player, Notification.Create(NotifEnum.OtherReceivesCard, player.Id));
    }

    /// <summary>
    ///     Checks if the player is allowed to exchange cards and asks him whether he wants to.
    /// </summary>
    private void TryExchangeCards(PlayerBase player)
    {
        if (!player.Hand.IsExchangeable() || player.Exchanged) return;
        
        player.Notify(Notification.Create(NotifEnum.AskExchange));
        if (!player.GetResponse<bool>().Data) return;

        player.Hand.Clear();

        var newCard = deck.Draw();
        player.Hand.Add(newCard);

        player.Notify(Notification.Create(NotifEnum.ReceivedCard, newCard));
        table.NotifyAllExcept(player, Notification.Create(NotifEnum.OtherExchanged, player));
    }

    /// <summary>
    ///    Evaluates the round and updates the game state.
    ///    Mostly informs the players about the results and updates the bank and player balances.
    /// </summary>
    /// <remarks> Does also work for the duel. </remarks>
    private void Evaluate()
    {
        if (table.Banker!.Hand.IsBust())
        {
            HandleBankerBust();
        }
        else
        {
            HandlePlayerWinsAndLosses();
        }

        UpdateBank();
        ResetPlayerStates();

        // To show the results to the players
        Thread.Sleep(1000);

        UpdateGameStateForAll();
    }

    /// <summary>
    ///     All players win.
    /// </summary>
    private void HandleBankerBust()
    {
        table.AllPlayers.ForEach(player =>
        {
            player.Balance += 2 * player.Bet;
            player.Bet = 0;
        });

        table.NotifyPlayers(Notification.Create(NotifEnum.Won));
    }

    private void HandlePlayerWinsAndLosses()
    {
        foreach (var player in table.AllPlayers)
        {
            var playerBestValue = player.Hand.GetBestValue();
            var bankerBestValue = table.Banker!.Hand.GetBestValue();

            if (playerBestValue > bankerBestValue && !player.Hand.IsBust())
            {
                player.Balance += 2 * player.Bet;
                player.Bet = 0;

                player.Notify(Notification.Create(NotifEnum.Won));
            }
            else if (player.Id != table.Banker.Id)
            {
                player.Notify(Notification.Create(NotifEnum.Lost));
            }
        }
    }

    /// <summary>
    ///     Collects the remaining bets from the players and adds them to the bank.
    /// </summary>
    private void UpdateBank()
    {
        table.Bank += 2 * table.AllPlayers.Sum(player => player.Bet);
    }
    
    private void ResetPlayerStates()
    {
        table.AllPlayers.ForEach(player => player.Bet = 0);
        table.AllPlayers.ForEach(player => player.Hand.Clear());
        table.AllPlayers.ForEach(player => player.Exchanged = false);
    }
    
    /// <summary>
    ///     Updates the game state for all players.
    ///     Usuallty it could be replaced by series of updates for each player but this is more convenient.
    /// </summary>
    private void UpdateGameStateForAll()
    {
        foreach (var player in table.AllPlayers)
        {
            player.Notify(Notification.Create(NotifEnum.UpdateGameState, CreateGameState(player)));
        }
    }

    /// <summary>
    ///     Creates a game state for a player.
    ///     The player is need to hide the other players' hands.
    /// </summary>
    /// <param name="player"> Player to show his hand</param>
    /// <returns>GameState struct</returns>
    private GameState CreateGameState(PlayerBase player)
    {
        return new GameState(table.AllPlayers, table.Bank, player.Hand);
    }
}