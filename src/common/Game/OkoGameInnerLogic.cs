using System.Diagnostics;
using OkoCommon.Communication;

namespace OkoCommon.Game;

public partial class OkoGame
{
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

            var lost = table.Banker!.Hand.GetBestValue() >= duelPlayer.Hand.GetBestValue() && !table.Banker.Hand.IsBust();
            table.NotifyAllExcept(duelPlayer,
                Notification.Create(lost ? NotifEnum.OtherLost : NotifEnum.OtherWins, duelPlayer.Name));
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
            TryExchangeCards(player);
            
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
        }
    }

    /// <summary>
    ///     Draw a card for the player and notify all players.
    /// </summary>
    private bool Bet(PlayerBase player, bool noBet = false)
    {
        if (noBet) return false;

        var bet = player.GetResponse<int>().Data;
        if (bet > player.Balance) return false;

        player.Bet = bet;
        player.Balance -= bet;
        table.Bank -= bet;

        UpdateGameStateForAll();

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
            TryExchangeCards(table.Banker);

            table.Banker.Notify(Notification.Create(NotifEnum.AskTurnNoBet));
            var decision = table.Banker.GetResponse<TurnDecision>().Data;

            if (decision == TurnDecision.Stop) return;

            var card = deck.Draw();
            table.Banker!.Hand.Add(card);

            table.Banker.Notify(Notification.Create(NotifEnum.ReceivedCard, card));
            table.NotifyAllExcept(table.Banker, Notification.Create(NotifEnum.OtherReceivesCard, table.Banker.Id));

            if (table.Banker.Hand.IsBust()) return;
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

        UpdateGameStateForAll();
    }

    /// <summary>
    ///     Evaluates the round and updates the game state.
    ///     Mostly informs the players about the results and updates the bank and player balances.
    /// </summary>
    /// <remarks> Does also work for the duel. </remarks>
    private void Evaluate()
    {
        if (table.Banker!.Hand.IsBust())
            HandleBankerBust();
        else
            HandlePlayerWinsAndLosses();

        UpdateBank();
        ResetPlayerStates();

        // To show the results to the players
        Thread.Sleep(1000);
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
            else if (player.Id != table.Banker.Id
                     && !player.Hand.IsBust() // busted has been handled already
                     && player.Bet > 0)
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
            player.Notify(Notification.Create(NotifEnum.UpdateGameState, CreateGameState(player)));
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