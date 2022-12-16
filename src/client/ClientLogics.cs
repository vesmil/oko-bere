using OkoCommon.Communication;
using OkoCommon.Game;

namespace OkoClient;

public class ClientPlayerLogics : IPlayerLogics
{
    private readonly Client Client;
    bool isBanker = false;

    public ClientPlayerLogics(Client client)
    {
        Client = client;
    }

    public void PlayerLoop()
    {
        while (true)
        {
            var update = Client.ReceiveNotification<object>();
            Console.WriteLine($"Type: {update?.Type.GetType().GetEnumName(update.Type)}");

            if (update?.Data != null)
            {
                Console.WriteLine($"Data: {update.Data}");
            }

            if (update is { Type: NotifEnum.EndOfGame })
            {
                break;
            }
            
            
            switch (update?.Type)
            {
                case NotifEnum.AskForName:
                    OnAskForName();
                    break;
                
                case NotifEnum.AskForTurn:
                    OnAskForTurn();
                    break;
                
                case NotifEnum.NewBanker:
                    // TODO
                    if (update.Data is string bankerName && Client.Name == bankerName)
                    {
                        isBanker = true;
                    }
                    break;
                
                default:
                    break;
            }
        }
    }

    public void OnAskForName()
    {
        if (Client.Name == "")
        {
            // TODO
            
            /*
            Console.Write("Enter your name: ");
            var name = Console.ReadLine() ?? string.Empty;
            Console.WriteLine(); 
            */
            
            Client.Name = new Random().Next().ToString();
        }

        Client.SendGenericResponse(Client.Name);
    }

    public void OnAskForTurn()
    {
        Console.WriteLine("It's your move, (d,b x,s)");
        var response = Console.ReadLine() ?? string.Empty;

        if (isBanker)
        {
            OnAskForTurnBanker();
            return;
        }
        
        if (response == "d")
        {
            Client.SendGenericResponse(PlayerResponseEnum.Draw);
        }
        else if (response.Trim()[0] == 'b')
        {
            Client.SendGenericResponse(PlayerResponseEnum.Bet);
            Client.SendGenericResponse(int.Parse(response.Trim()[1..]));
        }
        else if (response == "s")
        {
            Client.SendGenericResponse(PlayerResponseEnum.Stop);
        }
    }
    
    public new void OnAskForTurnBanker()
    {
        Console.WriteLine("It's your move, (d,s)");
        var response = Console.ReadLine() ?? string.Empty;
    }

    public void OnAskToCut()
    {
        Console.WriteLine("Where do you want to cut? (0-32)");
        var response = Console.ReadLine() ?? string.Empty;
        var cut = int.Parse(response);
        Client.SendGenericResponse(cut);
    }

    public void OnAskForContinue()
    {
        Console.WriteLine("Do you want to continue? (y/n)");
        var response = Console.ReadLine() ?? string.Empty;
        Client.SendGenericResponse(response == "y");
    }
    
    private void OnAskForMalaDomu()
    {
        Console.WriteLine("Do you want Mala domu? (y/n)");
        var answer = Console.ReadLine();
        Client.SendGenericResponse(answer == "y");
    }

    public void OnAskForInitialBank()
    {
        Console.WriteLine("How much money do you want to put in the bank?");
        var response = Console.ReadLine() ?? string.Empty;
        var bank = int.Parse(response);
        Client.SendGenericResponse(bank);
    }

    public void OnAskForDuel()
    {
        Console.WriteLine("Do you want to duel?");
        var response = Console.ReadLine() ?? string.Empty;
        Client.SendGenericResponse(response == "y");
    }
}