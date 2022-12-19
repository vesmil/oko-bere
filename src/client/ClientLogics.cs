using OkoCommon.Communication;
using OkoCommon.Game;

namespace OkoClient;

public class ClientPlayerLogics : IPlayerLogics
{
    private readonly Client client;
    private bool isBanker;

    public ClientPlayerLogics(Client client)
    {
        this.client = client;
    }

    public void PlayerLoop()
    {
        while (true)
        {
            // TODO this will be a lot different
            
            var update = client.ReceiveNotification<object>();
            Console.WriteLine($"{client.Name} - Type: {update?.Type.GetType().GetEnumName(update.Type)}");

            if (update?.Data != null)
            {
                Console.WriteLine($"{client.Name} - Data: {update.Data}");
            }
            
            Console.WriteLine();
            
            switch (update?.Type)
            {
                case NotifEnum.AskForName:
                    OnAskForName();
                    break;
                
                case NotifEnum.AskForTurn:
                    OnAskForTurn();
                    break;
                
                case NotifEnum.NewBanker:
                    if (update.Data is string bankerName && client.Name == bankerName)
                    {
                        isBanker = true;
                    }
                    
                    break;
                
                case NotifEnum.AskForMalaDomu:
                    OnAskForMalaDomu();
                    break;
                
                case NotifEnum.EndOfGame:
                    // ...
                    return;

                case NotifEnum.ChooseCutPlayer:
                    OnChooseCutPlayer();
                    break;
                
                case NotifEnum.ChooseCutPosition:
                    OnAskToCut();
                    break;

                // Choose cut player
            }
        }
    }

    private void OnChooseCutPlayer()
    {
        Console.WriteLine("You are asked to choose cut player...");
        Console.WriteLine("But its not done yet");
        // TODO
    }

    public void OnAskForName()
    {
        if (client.Name == "")
        {
            Console.Write("Enter your name: ");
            var name = Console.ReadLine() ?? string.Empty;
            Console.WriteLine();
        }

        client.SendGenericResponse(client.Name);
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
            client.SendGenericResponse(PlayerResponseEnum.Draw);
        }
        else if (response.Length > 0 && response.Trim()[0] == 'b')
        {
            client.SendGenericResponse(PlayerResponseEnum.Bet);
            client.SendGenericResponse(int.Parse(response.Trim()[1..]));
        }
        else if (response == "s")
        {
            client.SendGenericResponse(PlayerResponseEnum.Stop);
        }
    }

    private void OnAskForTurnBanker()
    {
        Console.WriteLine("It's your move, (d,s)");
        var response = Console.ReadLine() ?? string.Empty;
        
        switch (response)
        {
            case "d":
                client.SendGenericResponse(PlayerResponseEnum.Draw);
                break;
            case "s":
                client.SendGenericResponse(PlayerResponseEnum.Stop);
                break;
        }
    }

    public void OnAskToCut()
    {
        Console.WriteLine("Where do you want to cut? (0-32)");
        var response = Console.ReadLine() ?? string.Empty;
        var cut = int.Parse(response);
        client.SendGenericResponse(cut);
    }

    public void OnAskForContinue()
    {
        Console.WriteLine("Do you want to continue? (y/n)");
        var response = Console.ReadLine() ?? string.Empty;
        client.SendGenericResponse(response == "y");
    }
    
    private void OnAskForMalaDomu()
    {
        Console.WriteLine("Do you want Mala domu? (y/n)");
        var answer = Console.ReadLine();
        client.SendGenericResponse(answer == "y");
    }

    public void OnAskForInitialBank()
    {
        Console.WriteLine("How much money do you want to put in the bank?");
        var response = Console.ReadLine() ?? string.Empty;
        var bank = int.Parse(response);
        client.SendGenericResponse(bank);
    }

    public void OnAskForDuel()
    {
        Console.WriteLine("Do you want to duel?");
        var response = Console.ReadLine() ?? string.Empty;
        client.SendGenericResponse(response == "y");
    }
}