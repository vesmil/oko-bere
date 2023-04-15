using System.Diagnostics;
using OkoCommon.Communication;

namespace OkoCommon.Game;

public partial class OkoGame
{
    /// <summary>
    ///     Class defining a game operations related to a table and storing the state of the game, etc.
    /// </summary>
    private class GameTable
    {
        public List<PlayerBase> AllPlayers;
        public int Bank;

        private PlayerBase? bankBrokePlayer;
        public PlayerBase? Banker;
        public int InitialBank;

        public GameTable(List<PlayerBase> allPlayers)
        {
            AllPlayers = allPlayers;
            ClearBets();
        }

        /// <summary>
        ///     The players that are not the banker.
        /// </summary>
        public IEnumerable<PlayerBase> Players => AllPlayers.Where(p => !p.IsBanker);

        private IEnumerable<PlayerBase> AllExcept(PlayerBase player)
        {
            return AllPlayers.Where(p => p != player);
        }

        public void NotifyAllPlayers<T>(INotification<T> notification)
        {
            foreach (var player in AllPlayers) player.Notify(notification);
        }

        public void NotifyPlayers<T>(INotification<T> notification)
        {
            foreach (var player in Players) player.Notify(notification);
        }


        public void NotifyAllExcept<T>(PlayerBase except, INotification<T> notification)
        {
            foreach (var player in AllExcept(except)) player.Notify(notification);
        }

        internal void SetBanker()
        {
            if (AllPlayers.Count == 0)
            {
                Debug.WriteLine("No players, can not assign a banker");
                return;
            }

            if (Banker is not null) Banker.IsBanker = false;

            if (bankBrokePlayer is not null)
            {
                AssignBanker(bankBrokePlayer);
                bankBrokePlayer = null;
            }
            else
            {
                var num = new Random().Next(AllPlayers.Count);

                AssignBanker(AllPlayers[num]);
                NotifyAllPlayers(Notification.Create(NotifEnum.NewBanker, Banker?.ToPlayerInfo()));
            }

            Debug.WriteLine($"Banker was set to {Banker!.Name}");
        }

        /// <summary>
        ///     Assigns a new banker and notifies all players.
        /// </summary>
        private void AssignBanker(PlayerBase newBanker)
        {
            Banker = newBanker;
            Banker.IsBanker = true;

            Banker.Notify(Notification.Create(NotifEnum.AskInitialBank));
            InitialBank = Banker.GetResponse<int>().Data;

            Bank = InitialBank;
            NotifyAllPlayers(Notification.Create(NotifEnum.SetInitialBank, Bank));
        }

        private void ClearBets()
        {
            foreach (var player in AllPlayers) player.Bet = 0;
        }


        public bool AskForContinue()
        {
            NotifyAllPlayers(Notification.Create(NotifEnum.AskContinue));

            var newPlayers = WouldContinue(); // NOTE might do async...
            if (newPlayers.Count < 3)
            {
                NotifyAllPlayers(Notification.Create(NotifEnum.NotEnoughPlayers));
                return false;
            }

            foreach (var player in AllPlayers.Where(p => !newPlayers.Contains(p))) RemovePlayer(player);

            NotifyAllPlayers(Notification.Create(NotifEnum.Continue));

            return true;
        }

        private List<PlayerBase> WouldContinue()
        {
            return AllPlayers.Where(player => player.GetResponse<bool>().Data).ToList();
        }

        private void RemovePlayer(PlayerBase player)
        {
            AllPlayers.Remove(player);

            foreach (var otherPlayer in AllPlayers)
                otherPlayer.Notify(Notification.Create(NotifEnum.PlayerLeft, player.ToPlayerInfo()));
        }

        public void UpdatePlayers(List<PlayerBase> newPlayers)
        {
            foreach (var player in newPlayers.Where(player => !AllPlayers.Contains(player)))
                NotifyAllPlayers(Notification.Create(NotifEnum.NewPlayer, player.ToPlayerInfo()));

            AllPlayers = newPlayers;

            ClearBets();
        }
    }
}