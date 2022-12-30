using OkoCommon.Communication;
using OkoCommon.Game;

namespace OkoClient;

public class ClientLogics
{
    private readonly Client client;
    
    private bool isBanker;
    private string NamePreset { get; set; } = "";
    
    public event EventHandler<MessageReceivedEventArgs>? MessageReceived;

    public ClientLogics(string ip, int port)
    {
        client = new Client();
        client.Connect(ip, port);
        
        // will ask for name...
    }
    
    public ClientLogics(string name, string ip, int port)
    {
        client = new Client();
        client.Connect(ip, port);
        
        // TODO... on connect it will receive gameState and will give its name...
    }

    /// <summary>
    /// Main loop to receive messages from server
    /// </summary>
    public void PlayerLoop()
    {
        while (true)
        {
            var update = client.ReceiveNotification<object>();
            
            // TODO remove debug
            Console.WriteLine($"{NamePreset} - Type: {update?.Type.GetType().GetEnumName(update.Type)}");

            if (update?.Data != null)
            {
                Console.WriteLine($"{NamePreset} - Data: {update.Data}");
            }
            
            Console.WriteLine();
            
            if (update != null)
            {
                MessageReceived?.Invoke(this, new MessageReceivedEventArgs(update.Data, update.Type));
            }
            
            Thread.Sleep(100);
        }
    }
    
    // Bet, draw stop...
    
    // ChooseCutPlayer
    
    // ChooseCutCard 
    
    // AcceptDuel
}