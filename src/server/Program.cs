using System.Net;
using System.Net.Sockets;
using OkoCommon.Game;

namespace OkoServer;

public static class Program
{
    public static void Main(string[] args)
    {
        var selectedPort = 1234;
        if (args.Length > 0)
        {
            if (int.TryParse(args[0], out var port))
                selectedPort = port;
            else
            {
                Console.WriteLine("Usage: server [port] (default is 1234)");
                return;
            }
        }

        using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0);
        socket.Connect("8.8.8.8", 65530);
        var endPoint = socket.LocalEndPoint as IPEndPoint;
        var ip = endPoint?.Address.ToString();

        Console.WriteLine("Server IP: " + ip);

        var server = new Server(selectedPort);

        var serverThread = new Thread(server.AcceptLoop);
        serverThread.Start();

        var oko = new OkoGame(server.GetClients);
        server.AssignGame(oko);

        oko.Lobby();
        oko.GameLoop();
    }
}