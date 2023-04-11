using OkoCommon.Game;

namespace OkoServer;

public static class Program
{
    public static void Main()
    {
        // TODO add arg parsing - port
        // TODO add prints - ip address mainly
        
        var server = new Server();

        var serverThread = new Thread(server.AcceptLoop);
        serverThread.Start();

        var oko = new Game(server.GetClients);
        server.AssignGame(oko);

        oko.Lobby();
        oko.GameLoop();
    }
}