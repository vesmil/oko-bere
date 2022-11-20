namespace OkoServer;

public static class Program
{
    public static void Main(string[] args)
    {
        var server = new Server();

        // var thread = new Thread(server.AcceptLoop);
        // thread.Start();
        
        server.AcceptLoop();

        var players = server.GetPlayers();

        var oko = new OkoBere(players);
        oko.Start();
    }
}