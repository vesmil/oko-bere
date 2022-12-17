using OkoCommon;
using OkoCommon.Game;

namespace OkoServer;

public static class Program
{
    public static void Main(string[] args)
    {
        var server = new Server();

        // TODO
        // var thread = new Thread(server.AcceptLoop);
        // thread.Start();
        
        server.AcceptLoop();

        var players = server.GetPlayers();

        var oko = new Game(players);
        oko.GameLoop();
    }
}