using OkoCommon;
using OkoCommon.Interface;

namespace OkoCommon;

public class OkoBere
{
    private PlayerBase banker;
    private readonly List<PlayerBase> players;
    
    private IEnumerable<PlayerBase> allPlayers
    {
        get
        {
            foreach (var player in players) yield return player;
            yield return banker;
        }
    }

    private readonly List<(PlayerBase, int)> currentBets = new();

    private int bank;
    private int initialBank;

    private bool endPending;
    
    private readonly Deck deck = new();

    public OkoBere(List<PlayerBase> players)
    {
        banker = players[0];
        this.players = players;
    }

    public void Start()
    {
        // Select banker
        
        // Set initial bank

        while (bank > 0) OneRound();

        // Would you like to play again?
    }
    
    private void OneRound()
    {
        // Ask about "Malá domů" in case...
        
        deck.Shuffle();
        
        // Ignore players with no money?
        
        foreach (var player in players)
        {
            player.Hand.Clear();
            player.Hand.Add(deck.Draw());
            player.Exchanged = false;
        }

        // Cards have been dealt
        
        if (endPending && banker.Hand[0].Rank is Rank.King or Rank.Eight)
        {
            banker.Balance += bank;
            bank = 0;
            
            return;
        }

        foreach (var player in allPlayers)
        {
            PlayersTurn(player);
        }

        Evaluation();
    }

    private static void PlayersTurn(PlayerBase player)
    {
        // Check if is banker
        
        // Ask player for decision
        
        // In case of next or bet - deal cards, check bust and do it again
    }

    private void DrawCard(PlayerBase player)
    {
        var card = deck.Draw();
        player.Hand.Add(card);

        // Inform about the change
        // - player will get the card
        // - others will get the information about the card
    }

    private void ExchangeCards(PlayerBase player)
    {
        if (player.Exchanged) return;
        
        // Test if player is able to exchange cards
    }
    
    private void Evaluation()
    {
        // Get the results and split the bank accordingly
    }
}