using OkoCommon.Communication;

namespace OkoCommon.Game;

public partial class Game
{
    private class GameTable
    {
        public readonly Dictionary<PlayerBase, int> CurrentBets;
        public readonly List<PlayerBase> Players;

        public int Bank;
        public PlayerBase Banker;
        public int InitialBank;
        
        private PlayerBase? bankBrokePlayer;

        public GameTable(List<PlayerBase> players)
        {
            Banker = players[0];
            Players = players;

            CurrentBets = new Dictionary<PlayerBase, int>();
            ClearBets();
        }

        private IEnumerable<PlayerBase> AllPlayers => Players.Append(Banker);

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
            // Banker is either the one who took bank or raffled
            if (bankBrokePlayer is not null)
            {
                AssignBanker(bankBrokePlayer);
                bankBrokePlayer = null;
            }
            else
            {
                if (!Players.Contains(Banker)) Players.Add(Banker);
                var num = new Random().Next(Players.Count);

                // Might add animation for the raffle here
                
                AssignBanker(Players[num]);
                NotifyAllPlayers(new PlayerNotif(NotifEnum.NewBanker, Banker));
            }
            
            Console.WriteLine($"Banker was set to {Banker.Name}");
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
            // This should be zero, right?...
            foreach (var player in Players) CurrentBets[player] = 0;
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

            foreach (var player in Players.Where(p => !newPlayers.Contains(p)))
            {
                RemovePlayer(player);
            }
            
            NotifyAllPlayers(new NoDataNotif(NotifEnum.Continue));

            return true;
        }

        private List<PlayerBase> WouldContinue() => AllPlayers.Where(player => player.GetResponse<bool>().Data).ToList();
        
        private void RemovePlayer(PlayerBase player)
        {
            Players.Remove(player);
            CurrentBets.Remove(player);

            foreach (var otherPlayer in AllPlayers)
            {
                otherPlayer.Notify(new PlayerNotif(NotifEnum.PlayerLeft, player));
            }
        } 
    }
}