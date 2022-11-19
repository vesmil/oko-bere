using System.Net.Sockets;
using System.Text;
using OkoServer.Interface;

namespace OkoServer;

public class TcpPlayer : IPlayer
{
    public IAction GetAction()
    {
        var buffer = new byte[1024];
        var bytesRead = stream.Read(buffer, 0, buffer.Length);
                    
        if (bytesRead > 0)
        {
            var message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Console.WriteLine("Received: " + message);
        }
        
        return new Action();
    }

    public bool SendAction(IAction action)
    {
        var data = Encoding.UTF8.GetBytes("Action");
        stream.Write(data, 0, data.Length);
        
        if (stream.CanRead)
        {
            var buffer = new byte[1024];
            var bytesRead = stream.Read(buffer, 0, buffer.Length);
            var response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            return response == "OK";
        }
        
        return false;
    }

    NetworkStream stream;
    
    public TcpPlayer(NetworkStream stream)
    {
        this.stream = stream;
    }
}