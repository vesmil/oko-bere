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
        messageHandlers.Add(NotifEnum.NewBanker, HandleNewBanker);
        messageHandlers.Add(NotifEnum.AskInitialBank, HandleAskInitialBank);
        messageHandlers.Add(NotifEnum.SetInitialBank, HandleSetInitialBank);
        messageHandlers.Add(NotifEnum.BankBusted, _ => SetTurnInfo("Bank was busted!"));

        messageHandlers.Add(NotifEnum.AskMalaDomu, HandleAskMalaDomu);
        messageHandlers.Add(NotifEnum.MalaDomuCalled, HandleMalaDomuCalled);
        messageHandlers.Add(NotifEnum.MalaDomuSuccess, HandleMalaDomuSuccess);

        messageHandlers.Add(NotifEnum.AskChooseCutPlayer, HandleChooseCutPlayer);
        messageHandlers.Add(NotifEnum.AskChooseCutPosition, HandleChooseCutPosition);

        messageHandlers.Add(NotifEnum.ShowCutCard, HandleSeeCutCard);
        messageHandlers.Add(NotifEnum.AskTurn, HandleAskForTurn);
        messageHandlers.Add(NotifEnum.AskDuel, HandleDuelOffer);
        messageHandlers.Add(NotifEnum.AskTurnNoBet, HandleAskNextNoBet);
        messageHandlers.Add(NotifEnum.AskContinue, HandleAskForContinue);
        messageHandlers.Add(NotifEnum.AskExchange, HandleAskForExchange);

        messageHandlers.Add(NotifEnum.NotEnoughPlayers, HandleNotEnoughPlayers);
        messageHandlers.Add(NotifEnum.EndOfGame, HandleEndOfGame);

        messageHandlers.Add(NotifEnum.Bust, _ => SetTurnInfo("You busted!"));
        messageHandlers.Add(NotifEnum.Won, _ => SetTurnInfo("You won!"));
        messageHandlers.Add(NotifEnum.Lost, _ => SetTurnInfo("You lost!"));

        messageHandlers.Add(NotifEnum.OtherBusts, msg => SetTurnInfo($"{msg.Data} busted!"));
        messageHandlers.Add(NotifEnum.OtherWins, msg => SetTurnInfo($"{msg.Data} wins!"));
        messageHandlers.Add(NotifEnum.OtherLost, msg => SetTurnInfo($"{msg.Data} lost!"));
        messageHandlers.Add(NotifEnum.OtherDuel, msg => SetTurnInfo($"{msg.Data} started duel!"));
    }

    private void HandleAskMalaDomu(MessageReceivedEventArgs obj)
    {
        // TODO show dialog
    }

    private void HandleAskForExchange(MessageReceivedEventArgs obj)
    {
        // TODO show dialog
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
        int.TryParse(buttonPanel.BetTextBox.Text, out var bet);
        if (bet <= 0 || bet > GameState.Players.First(p => p.Id == client.PlayerId).Balance)
        {
            MessageBox.Show("Bet amount not valid");
            return;
        }

        client.Duel(bet);
        buttonPanel.HideAll();
    }

    private void DeclineDuel()
    {
        client.Duel(0);
        buttonPanel.HideAll();
    }

    private void HandleAskNextNoBet(MessageReceivedEventArgs _)
    {
        SetTurnInfo("Would you like to get another card?");
        buttonPanel.NoBet();
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
        Dispose();
    }

    private void ContinueButtonClick(object sender, EventArgs e)
    {
        client.Continue(true);

        buttonPanel.ContinueButton.Hide();
        SetTurnInfo("Waiting for other players...");
        timer.Stop();
    }

    private void DrawButtonClick(object sender, EventArgs e)
    {
        client.Turn(TurnDecision.Draw);
        buttonPanel.HideAll();
    }

    private void BetButtonClick(object sender, EventArgs e)
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

    private void EndTurnButtonClick(object sender, EventArgs e)
    {
        client.Turn(TurnDecision.Stop);
        buttonPanel.HideAll();
    }
}