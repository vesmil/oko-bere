using OkoCommon.Game;

namespace OkoServer;

public static class Program
{
    public static void Main(string[] args)
    {
        var server = new Server();

        var serverThread = new Thread(server.AcceptLoop);
        serverThread.Start();

        var oko = new Game(server.GetPlayers);
        oko.GameLoop();
    }
}