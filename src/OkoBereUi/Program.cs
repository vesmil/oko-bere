using System.Net;
using System.Net.Sockets;
using OkoClient;
using OkoCommon.Game;
using OkoServer;

namespace OkoBereUi
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            var server = new Server();
        
            var serverThread = new Thread(server.AcceptLoop);
            serverThread.Start();
        
            Thread.Sleep(500);
        
            var client = new Client();
            client.PresetName("Alice");
            ConnectToSelf(client);
        
            var client2 = new Client();
            client2.PresetName("Bob");
            ConnectToSelf(client2);

            Thread.Sleep(500);

            
            ApplicationConfiguration.Initialize();
            Application.Run(new GameTableForm(client));

            var ui2 = new GameTableForm(client2);
            ui2.Show();
            
            // TODO Show UI in two threads
            
            var player1 = new ClientPlayerLogics(client);
            var player2 = new ClientPlayerLogics(client2);
        
            var playerThread1 = new Thread(player1.PlayerLoop);
            var playerThread2 = new Thread(player2.PlayerLoop);
        
            playerThread1.Start();
            playerThread2.Start();

            var oko = new Game(server.GetPlayers);
        
            while (server.GetPlayers().Count < 2) Thread.Sleep(200);
        
            oko.GameLoop();
        }

        private static void ConnectToSelf(Client client)
        {
            using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0);
            socket.Connect("8.8.8.8", 65530);
            var endPoint = socket.LocalEndPoint as IPEndPoint;
            var ip = endPoint?.Address.ToString();

            if (ip != null) client.Connect(ip, 1234);
        }
    }
}