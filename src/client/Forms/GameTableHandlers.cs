using OkoClient.Client;
using OkoCommon;
using OkoCommon.Communication;
using OkoCommon.Game;

namespace OkoClient.Forms;

public sealed partial class GameTableForm
{
    private readonly Dictionary<NotifEnum, Action<MessageReceivedEventArgs>> messageHandlers = new();

    private void InitializeHandlers()
    {
        // messageHandlers.Add(NotifEnum.GameStart, HandleGameStart);
        messageHandlers.Add(NotifEnum.NewBanker, HandleNewBanker);
        messageHandlers.Add(NotifEnum.AskInitialBank, HandleAskInitialBank);
        messageHandlers.Add(NotifEnum.SetInitialBank, HandleSetInitialBank);
        messageHandlers.Add(NotifEnum.BankBusted, HandleBankBusted);
        messageHandlers.Add(NotifEnum.AskTurn, HandleAskForTurn);
        messageHandlers.Add(NotifEnum.MalaDomuCalled, HandleMalaDomuCalled);
        messageHandlers.Add(NotifEnum.MalaDomuSuccess, HandleMalaDomuSuccess);
        messageHandlers.Add(NotifEnum.AskChooseCutPlayer, HandleChooseCutPlayer);
        messageHandlers.Add(NotifEnum.AskChooseCutPosition, HandleChooseCutPosition);
        messageHandlers.Add(NotifEnum.ShowCutCard, HandleSeeCutCard);
        messageHandlers.Add(NotifEnum.AskDuel, HandleDuelOffer);
        messageHandlers.Add(NotifEnum.DuelDeclined, HandleDuelDeclined);
        messageHandlers.Add(NotifEnum.DuelAccepted, HandleDuelAccepted);
        messageHandlers.Add(NotifEnum.AskTurnNoBet, HandleDuelAskNextCard);
        messageHandlers.Add(NotifEnum.AskContinue, HandleAskForContinue);
        messageHandlers.Add(NotifEnum.NotEnoughPlayers, HandleNotEnoughPlayers);
        messageHandlers.Add(NotifEnum.EndOfGame, HandleEndOfGame);
    }
    
    private void HandleNewBanker(MessageReceivedEventArgs msg)
    {
        if (msg.Data is PlayerInfo playerInfo)
            if (playerInfo.Id == client.PlayerId)
                SetTurnInfo("You are the new banker!");
            else
                SetTurnInfo("New banker was assigned - " + playerInfo.Name);
        else
            SetTurnInfo("New banker was assigned");
    }

    private void HandleGameStart(MessageReceivedEventArgs _)
    {
        SetTurnInfo("Game started!");
    }
    
    private void HandleAskInitialBank(MessageReceivedEventArgs obj)
    {
        SetTurnInfo("Set the initial bank, please");
        
        // TODO get initial bank
        
        client.BankSet(100);    
    }


    private void HandleSetInitialBank(MessageReceivedEventArgs _)
    {
        SetTurnInfo("Initial bank was set!");
        bankLabel.CheckInvoke(() => bankLabel.Text = "Bank : " + GameState.Bank);
    }

    private void HandleBankBusted(MessageReceivedEventArgs _)
    {
        SetTurnInfo("Bank was busted!");
    }

    private void HandleAskForTurn(MessageReceivedEventArgs _)
    {
        SetTurnInfo("It is your turn!");
        buttonPanel.Show();
    }

    private void HandleMalaDomuCalled(MessageReceivedEventArgs _)
    {
        SetTurnInfo("Banker called \"Mala domu\"!");
    }

    private void HandleMalaDomuSuccess(MessageReceivedEventArgs _)
    {
        SetTurnInfo("Mala domu success! New banker needs to be chosen");
    }

    private void HandleChooseCutPlayer(MessageReceivedEventArgs _)
    {
        SetTurnInfo("Choose the player to cut the deck");

        foreach (var playerBox in playerBoxes.Where(playerBox => playerBox.Player.Id != client.PlayerId))
        {
            playerBox.SelectButton.CheckInvoke(() =>
            {
                playerBox.SelectButton.Show();
            });
        }
    }
    
    private void SelectCutPlayer(Guid id)
    {
        client.CutPlayer(id);

        foreach (var button in playerBoxes.Select(box => box.Controls.OfType<Button>().FirstOrDefault()))
        {
            button?.CheckInvoke(() =>
            {
                button.Hide();
            });
        }
    }

    private void HandleChooseCutPosition(MessageReceivedEventArgs _)
    {
        // Let the player choose the position to cut the deck
        var random = new Random();
        client.Cut(random.Next(0, 32));
    }

    private void HandleSeeCutCard(MessageReceivedEventArgs message)
    {
        if (message.Data is Card card)
        {
            SetTurnInfo($"The cut card is {card}");
        }
        else
        {
            SetTurnInfo("The cut card is unknown - something went wrong");
        }
    }


    private void HandleDuelOffer(MessageReceivedEventArgs _)
    {
        SetTurnInfo(topLabel.Text + "\nWould you like to duel?");

        buttonPanel.Controls.Clear();

        // ...
        var acceptButton = new Button
        {
            Text = "Accept",
        };
        
        acceptButton.Click += (_, _) => RespondToDuel(100);

        var declineButton = new Button
        {
            Text = "Decline",
        };
        
        declineButton.Click += (_, _) => RespondToDuel(0);

        buttonPanel.Controls.Add(acceptButton);
        buttonPanel.Controls.Add(declineButton);
    }

    private void RespondToDuel(int bet)
    {
        client.Duel(bet);
        buttonPanel.Controls.Clear();
    }

    private void HandleDuelDeclined(MessageReceivedEventArgs _)
    {
        SetTurnInfo("Duel declined");
    }

    private void HandleDuelAccepted(MessageReceivedEventArgs _)
    {
        SetTurnInfo("Duel accepted");
    }

    private void HandleDuelAskNextCard(MessageReceivedEventArgs _)
    {
        SetTurnInfo("Would you like to get another card?");

        buttonPanel.Controls.Clear();

        var yesButton = new Button
        {
            Text = "Yes",
        };
        yesButton.Click += (_, _) => RequestNextCard(true);

        var noButton = new Button
        {
            Text = "No",
        };
        noButton.Click += (_, _) => RequestNextCard(false);

        buttonPanel.Controls.Add(yesButton);
        buttonPanel.Controls.Add(noButton);
    }

    private void RequestNextCard(bool request)
    {
        // client.NextCard(request);
        buttonPanel.Controls.Clear();
    }

    private void HandleAskForContinue(MessageReceivedEventArgs _)
    {
        ShowContinueButton();
    }

    private void HandleNotEnoughPlayers(MessageReceivedEventArgs _)
    {
        SetTurnInfo("Not enough players to continue");
    }

    private void HandleEndOfGame(MessageReceivedEventArgs _)
    {
        SetTurnInfo("Game ended");
        // TODO hide...
    }

}