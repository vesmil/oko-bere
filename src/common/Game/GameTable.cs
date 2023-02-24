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

        private IEnumerable<PlayerBase> AllPlayers => Banker is not null ? Players.Append(Banker) : Players;

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
                
                NotifyAllPlayers(new PlayerNotif(NotifEnum.NewBanker, Banker!));
            }

            Console.WriteLine($"Banker was set to {Banker!.Name}");
        }

        private void AssignBanker(PlayerBase newBanker)
        {
            Banker = newBanker;
            Players.Remove(Banker);

            Banker.Notify(new NoDataNotif(NotifEnum.SetInitialBank));

            InitialBank = 100;
            Bank = InitialBank;
        }

        private void ClearBets()
        {
            foreach (var player in Players) player.Bet = 0;
        }


        public bool AskForContinue()
        {
            NotifyAllPlayers(new NoDataNotif(NotifEnum.AskForContinue));

            var newPlayers = WouldContinue();

            if (newPlayers.Count < 3)
            {
                NotifyAllPlayers(new NoDataNotif(NotifEnum.NotEnoughPlayers));
                return false;
            }

            foreach (var player in Players.Where(p => !newPlayers.Contains(p))) RemovePlayer(player);

            NotifyAllPlayers(new NoDataNotif(NotifEnum.Continue));

            return true;
        }

        private List<PlayerBase> WouldContinue()
        {
            return AllPlayers.Where(player => player.GetResponse<bool>().Data).ToList();
        }

        private void RemovePlayer(PlayerBase player)
        {
            Players.Remove(player);

            foreach (var otherPlayer in AllPlayers) otherPlayer.Notify(new PlayerNotif(NotifEnum.PlayerLeft, player));
        }

        public void UpdatePlayers(List<PlayerBase> newPlayers)
        {
            foreach (var player in newPlayers.Where(player => !Players.Contains(player)))
            {
                NotifyAllPlayers(new PlayerNotif(NotifEnum.NewPlayer, player));
            }
            
            Players = newPlayers;
            
            ClearBets();
        }
    }
}