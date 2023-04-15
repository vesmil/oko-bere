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

        // NOTE will be later chosen by the banker...
        client.BankSet(GameState.Players.First(p => p.Id == client.PlayerId).Balance);
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
        buttonPanel.Turn();
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
            playerBox.SelectButton.CheckInvoke(() => { playerBox.SelectButton.Show(); });
    }

    private void SelectCutPlayer(Guid id)
    {
        client.CutPlayer(id);

        foreach (var button in playerBoxes.Select(box => box.Controls.OfType<Button>().FirstOrDefault()))
            button?.CheckInvoke(() => { button.Hide(); });
    }

    private void HandleChooseCutPosition(MessageReceivedEventArgs _)
    {
        var random = new Random();
        client.Cut(random.Next(0, 32));
    }

    private void HandleSeeCutCard(MessageReceivedEventArgs message)
    {
        if (message.Data is Card card)
            SetTurnInfo($"The cut card is {card}");
        else
            SetTurnInfo("The cut card is unknown - something went wrong");
    }

    private void HandleDuelOffer(MessageReceivedEventArgs _)
    {
        SetTurnInfo("Would you like to duel?");
        buttonPanel.Duel();
    }

    private void RespondToDuel()
    {
        client.Duel(int.TryParse(buttonPanel.BetTextBox.Text, out var bet) ? bet : 0);
        buttonPanel.HideAll();
    }

    private void DeclineDuel()
    {
        client.Duel(0);
        buttonPanel.HideAll();
    }

    // TODO not just duel but nobet
    private void HandleDuelAskNextCard(MessageReceivedEventArgs _)
    {
        SetTurnInfo("Would you like to get another card?");

        buttonPanel.HideAll();
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
        // TODO...
        Dispose();
    }

    // TODO move to button panel?
    private void ContinueButton_Click(object sender, EventArgs e)
    {
        client.Continue(true);

        buttonPanel.ContinueButton.Hide();
        SetTurnInfo("Waiting for other players...");
        timer.Stop();
    }

    private void DrawButton_Click(object sender, EventArgs e)
    {
        client.Turn(TurnDecision.Draw);
        buttonPanel.HideAll();
    }

    private void BetButton_Click(object sender, EventArgs e)
    {
        if (int.TryParse(buttonPanel.BetTextBox.Text, out var betAmount) && betAmount > 0 &&
            betAmount <= GameState.Players.First(p => p.Id == PlayerId).Balance)
        {
            client.Turn(TurnDecision.Bet, betAmount);
            buttonPanel.HideAll();
        }
        else
            MessageBox.Show("Bet amount not valid");
    }

    private void EndTurnButton_Click(object sender, EventArgs e)
    {
        client.Turn(TurnDecision.Stop);
        topLabel.Text = "Waiting for other players...";
        buttonPanel.HideAll();
    }
}