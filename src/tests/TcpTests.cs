using OkoClient;
using OkoServer;

namespace Tests;

public class TcpTests
{
    Server server = new();
    List<Client> clients = new();

    [SetUp]
    public void Setup()
    {
        // server.AcceptLoop();
        
        for (var i = 0; i < 10; i++)
        {
            // TODO...
        }
    }

    [Test]
    public void Test1()
    {
        Assert.Pass();
    }
}