using OkoCommon.Communication;

namespace OkoCommon.Game;

public partial class Game
{
    private class GameTable
    {
        public int Bank;

        private PlayerBase? bankBrokePlayer;
        public PlayerBase? Banker;
        public int InitialBank;
        public List<PlayerBase> Players;

        public GameTable(List<PlayerBase> players)
        {
            Players = players;
            ClearBets();
        }

        public IEnumerable<PlayerBase> AllPlayers => Banker is not null ? Players.Append(Banker) : Players;

        public IEnumerable<PlayerBase> AllExcept(PlayerBase player)
        {
            return AllPlayers.Where(p => p != player);
        }

        public void NotifyAllPlayers<T>(INotification<T> notification)
        {
            foreach (var player in AllPlayers) player.Notify(notification);
        }

        public void NotifyAllExcept<T>(PlayerBase except, INotification<T> notification)
        {
            foreach (var player in AllExcept(except)) player.Notify(notification);
        }

        internal void SetBanker()
        {
            if (Players.Count == 0)
            {
                Console.WriteLine("No players, can not assign a banker");
                return;
            }

            // Banker is either the one who took bank or raffled
            if (bankBrokePlayer is not null)
            {
                AssignBanker(bankBrokePlayer);
                bankBrokePlayer = null;
            }
            else
            {
                if (Banker is not null && !Players.Contains(Banker)) Players.Add(Banker);
                var num = new Random().Next(Players.Count);

                // Might add animation for the raffle here

                AssignBanker(Players[num]);

                NotifyAllPlayers(Notification.Create(NotifEnum.NewBanker, Banker?.ToPlayerInfo()));
            }

            Console.WriteLine($"Banker was set to {Banker!.Name}");
        }

        private void AssignBanker(PlayerBase newBanker)
        {
            Banker = newBanker;
            Players.Remove(Banker);

            Banker.Notify(Notification.Create(NotifEnum.SetInitialBank));

            InitialBank = 100;
            Bank = InitialBank;
        }

        private void ClearBets()
        {
            foreach (var player in Players) player.Bet = 0;
        }


        public bool AskForContinue()
        {
            NotifyAllPlayers(Notification.Create(NotifEnum.AskForContinue));

            var newPlayers = WouldContinue();

            /* TODO find use for async version
             
            var tasks = Players.Select(p => p.GetResponseAsync<bool>());
            var results = Task.WhenAll(tasks).Result;

            for (var i = 0; i < results.Length; i++)
            {
                if (!results[i]) Players.RemoveAt(i);
            }
            
            */

            if (newPlayers.Count < 3)
            {
                NotifyAllPlayers(Notification.Create(NotifEnum.NotEnoughPlayers));
                return false;
            }

            foreach (var player in Players.Where(p => !newPlayers.Contains(p))) RemovePlayer(player);

            NotifyAllPlayers(Notification.Create(NotifEnum.Continue));

            return true;
        }

        private List<PlayerBase> WouldContinue()
        {
            return AllPlayers.Where(player => player.GetResponse<bool>().Data).ToList();
        }

        private void RemovePlayer(PlayerBase player)
        {
            Players.Remove(player);

            foreach (var otherPlayer in AllPlayers)
                otherPlayer.Notify(Notification.Create(NotifEnum.PlayerLeft, player.ToPlayerInfo()));
        }

        public void UpdatePlayers(List<PlayerBase> newPlayers)
        {
            foreach (var player in newPlayers.Where(player => !Players.Contains(player)))
                NotifyAllPlayers(Notification.Create(NotifEnum.NewPlayer, player.ToPlayerInfo()));

            Players = newPlayers;

            ClearBets();
        }
    }
}