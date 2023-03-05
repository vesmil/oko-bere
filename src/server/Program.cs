using OkoCommon.Game;

namespace OkoServer;

public static class Program
{
    public static void Main()
    {
        var server = new Server();

        var serverThread = new Thread(server.AcceptLoop);
        serverThread.Start();

        var oko = new Game(server.GetClients);
        server.AssignGame(oko);

        oko.Lobby();
        oko.GameLoop();
    }
}