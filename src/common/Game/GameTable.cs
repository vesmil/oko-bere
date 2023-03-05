using System.Diagnostics;
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
        
        public List<PlayerBase> AllPlayers;
        public IEnumerable<PlayerBase> Players => AllPlayers.Where(p => !p.IsBanker);

        public GameTable(List<PlayerBase> allPlayers)
        {
            AllPlayers = allPlayers;
            ClearBets();
        }

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

        private void AssignBanker(PlayerBase newBanker)
        {
            Banker = newBanker;
            Banker.IsBanker = true;

            Banker.Notify(Notification.Create(NotifEnum.SetInitialBank));
            InitialBank = Banker.GetResponse<int>().Data;
            
            Bank = InitialBank;
        }

        private void ClearBets()
        {
            foreach (var player in AllPlayers) player.Bet = 0;
        }


        public bool AskForContinue()
        {
            NotifyAllPlayers(Notification.Create(NotifEnum.AskForContinue));

            var newPlayers = WouldContinue();  // NOTE might do async...
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