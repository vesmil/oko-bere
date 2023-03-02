﻿using OkoCommon.Communication;

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
    }

    public void Lobby()
    {
        // Notify that lobby is open
        
        while (true)
        {
            table.UpdatePlayers(getGetPlayersDelegate.Invoke());
            
            if (table.Players.Count > 2) 
                break;
            
            // Send all players a message to wait for more players
            // TODO...
            
            Thread.Sleep(200);
        }
    }
    
    public void GameLoop()
    {
        table.AskForContinue();

        while (true)
        {
            table.SetBanker();

            while (table.Bank > 0)
            {
                var newPlayers = getGetPlayersDelegate.Invoke();

                if (newPlayers.Count != table.Players.Count) table.UpdatePlayers(newPlayers);
                
                OneRound();
            }

            if (!table.AskForContinue()) break;
        }
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
                table.Banker.Notify(new NoDataNotif(NotifEnum.AskForMalaDomu));
                malaDomu = table.Banker.GetResponse<bool>().Data;
            }
            else
                throw new Exception("Banker is missing");

            if (malaDomu)
            {
                Console.WriteLine("Mala domu was called");
                table.NotifyAllPlayers(new NoDataNotif(NotifEnum.MalaDomuCalled));
            }
        }

        deck.Shuffle();
        Console.WriteLine("Deck was shuffled");

        PlayerBase? cutPlayer = null;
        while (cutPlayer is null)
        {
            table.Banker.Notify(new NoDataNotif(NotifEnum.ChooseCutPlayer));
            var cutPlayerName = table.Banker.GetResponse<string>().Data;
            cutPlayer = table.Players.FirstOrDefault(x => x.Name == cutPlayerName);
        }

        var duelInitiated = CutAndDuel(cutPlayer);

        if (duelInitiated) return;

        foreach (var player in table.Players.Where(p => p.Balance != 0).Append(table.Banker))
        {
            player.Hand.Clear();
            player.Hand.Add(deck.Draw());
            player.Exchanged = false;

            player.Notify(new CardNotif(NotifEnum.ReceivedCard, player.Hand[0]));
        }

        if (malaDomu && table.Banker.Hand[0].Rank is Rank.King or Rank.Eight)
        {
            table.NotifyAllPlayers(new NoDataNotif(NotifEnum.MalaDomuSuccess));

            table.Banker.Balance += table.Bank;
            table.Bank = 0;

            return;
        }

        foreach (var player in table.Players) PlayersTurn(player);

        BankersTurn();

        Evaluation();
    }

    private bool CutAndDuel(PlayerBase cutPlayer)
    {
        if (table.Banker is null) throw new Exception("Can not start duel without a banker.");

        var index = table.Players.IndexOf(cutPlayer);
        var duelPlayer = table.Players[(index + 1) % table.Players.Count];

        // Let the cutPlayer choose where to cut
        cutPlayer.Notify(new NoDataNotif(NotifEnum.ChooseCutPosition));
        var cutIndex = cutPlayer.GetResponse<int>().Data;

        var cutCard = deck.Cut(cutIndex);
        table.NotifyAllPlayers(new GenericNotif<Card>(NotifEnum.SeeCutCard, cutCard));

        duelPlayer.Notify(new NoDataNotif(NotifEnum.DuelOffer));
        var bet = duelPlayer.GetResponse<int>().Data;

        if (bet == 0) return false;

        table.Banker.Notify(new GenericNotif<int>(NotifEnum.DuelOffer, bet));
        var accept = table.Banker.GetResponse<bool>().Data;

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

        table.Banker!.Notify(new NoDataNotif(NotifEnum.DuelAskNextCard));
        // ...

        // Announce winner
    }

    private void PlayersTurn(PlayerBase player)
    {
        while (true)
        {
            player.Notify(new NoDataNotif(NotifEnum.AskForTurn));
            var decision = player.GetResponse<PlayerResponseEnum>().Data;

            switch (decision)
            {
                case PlayerResponseEnum.Bet:
                // ...
                case PlayerResponseEnum.Draw:
                    DrawCard(player);
                    break;
                case PlayerResponseEnum.Stop:
                    return;
            }

            if (player.Hand.IsBust())
            {
                player.Notify(new NoDataNotif(NotifEnum.Bust));
                break;
            }
        }
    }

    private void BankersTurn()
    {
        var card = deck.Draw();
        table.Banker!.Hand.Add(card);

        // Ask if he would like to make it visible

        // ...
    }

    private void DrawCard(PlayerBase player)
    {
        var card = deck.Draw();
        player.Hand.Add(card);

        // player.SendNotification(... , card);

        foreach (var otherPlayer in table.AllExcept(player))
        {
            // otherPlayer.SendNotification(...);
        }
    }

    private void ExchangeCards(PlayerBase player)
    {
        if (player.Exchanged) player.Notify(new NoDataNotif(NotifEnum.AlreadyExchanged));

        player.Hand.Clear();
        player.Hand.Add(deck.Draw());

        // Notify player about his new hand
        // ...
    }

    private void Evaluation()
    {
        // Inform the players...

        foreach (var player in table.Players.Where(player =>
                     player.Hand.GetBestValue() > table.Banker!.Hand.GetBestValue()))
        {
            player.Balance += 2 * player.Bet;
            player.Bet = 0;
        }

        table.Bank += table.Players.Sum(player => player.Bet);
        table.Players.ForEach(player => player.Bet = 0);
    }

    public void OnNewPlayer(PlayerBase newPlayer)
    {
        table.NotifyAllPlayers(new PlayerNotif(NotifEnum.NewPlayer, newPlayer));
        var gameState = CreateGameState();
        newPlayer.Notify(new GenericNotif<GameState>(NotifEnum.GameStateInfo, gameState));
    }
    
    private GameState CreateGameState()
    {
        var gameState = new GameState();

        foreach (var player in table.AllPlayers)
        {
            gameState.Players.Add(new PlayerInfo(player.Name, player.Balance, player.Bet, player.Hand.Count));
        }

        return gameState;
    }
}