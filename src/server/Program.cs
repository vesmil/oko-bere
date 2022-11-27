using OkoCommon;
using OkoCommon.Game;

namespace OkoServer;

public static class Program
{
    public static void Main(string[] args)
    {
        var server = new Server();

        // TODO in future the accepting will be open all time, and the player will be added next round
        
        // var thread = new Thread(server.AcceptLoop);
        // thread.Start();
        
        server.AcceptLoop();

        var players = server.GetPlayers();

        var oko = new Game(players);
        oko.GameLoop();
    }
}