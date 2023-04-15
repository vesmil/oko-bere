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
    // To ensure save adding of players
    private readonly Mutex addingMutex = new();
    private readonly Deck deck = new();

    // Delegate to get all player connected to the server - could also be used to separate different games
    private readonly GetPlayersDelegate getGetPlayersDelegate;
    private readonly GameTable table;

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
}