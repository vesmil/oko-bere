using OkoClient.Client;
using OkoCommon;
using OkoCommon.Communication;
using OkoCommon.Game;

namespace OkoClient.Forms;

public sealed partial class GameTableForm
{
    private readonly Dictionary<NotifEnum, Action<MessageReceivedEventArgs>> messageHandlers = new();

    public void InitializeHandlers()
    {
        messageHandlers.Add(NotifEnum.GameStart, HandleGameStart);
        messageHandlers.Add(NotifEnum.NewBanker, HandleNewBanker);
        messageHandlers.Add(NotifEnum.SetInitialBank, HandleSetInitialBank);
        messageHandlers.Add(NotifEnum.BankBusted, HandleBankBusted);
        messageHandlers.Add(NotifEnum.AskForTurn, HandleAskForTurn);
        messageHandlers.Add(NotifEnum.MalaDomuCalled, HandleMalaDomuCalled);
        messageHandlers.Add(NotifEnum.MalaDomuSuccess, HandleMalaDomuSuccess);
        messageHandlers.Add(NotifEnum.ChooseCutPlayer, HandleChooseCutPlayer);
        messageHandlers.Add(NotifEnum.ChooseCutPosition, HandleChooseCutPosition);
        messageHandlers.Add(NotifEnum.SeeCutCard, HandleSeeCutCard);
        messageHandlers.Add(NotifEnum.DuelOffer, HandleDuelOffer);
        messageHandlers.Add(NotifEnum.DuelDeclined, HandleDuelDeclined);
        messageHandlers.Add(NotifEnum.DuelAccepted, HandleDuelAccepted);
        messageHandlers.Add(NotifEnum.DuelAskNextCard, HandleDuelAskNextCard);
        messageHandlers.Add(NotifEnum.AskForContinue, HandleAskForContinue);
        messageHandlers.Add(NotifEnum.NotEnoughPlayers, HandleNotEnoughPlayers);
        messageHandlers.Add(NotifEnum.EndOfGame, HandleEndOfGame);
    }

    private void HandleNewBanker(MessageReceivedEventArgs msg)
    {
        if (msg.Data is PlayerInfo playerInfo)
            SetTurnInfo("New banker was assigned = " + playerInfo.Name);
        else
            SetTurnInfo("New banker was assigned");
    }

    private void HandleGameStart(MessageReceivedEventArgs _)
    {
        SetTurnInfo("Game started!");
    }

    private void HandleSetInitialBank(MessageReceivedEventArgs _)
    {
        SetTurnInfo("Set the initial bank, please");
        
        // TODO get initial bank
        
        client.BankSet(100);
    }

    private void HandleBankBusted(MessageReceivedEventArgs _)
    {
        SetTurnInfo("Bank was busted!");
    }

    private void HandleAskForTurn(MessageReceivedEventArgs _)
    {
        SetTurnInfo("It is your turn!");
        buttonPanel.Show();
        // ...
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
        // TODO ...
    }

    private void HandleChooseCutPosition(MessageReceivedEventArgs _)
    {
        SetTurnInfo("Where would you like to cut the deck?");
        // TODO ...
    }

    private void HandleSeeCutCard(MessageReceivedEventArgs message)
    {
        if (message.Data is Card card)
        {
            SetTurnInfo($"The card is {card}");
        }
        else
        {
            SetTurnInfo("The card is unknown");
        }
    }

    private void HandleDuelOffer(MessageReceivedEventArgs _)
    {
        SetTurnInfo("Would you like to duel?");
        // TODO ...
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
        // TODO ...
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