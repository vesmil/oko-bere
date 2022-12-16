namespace OkoCommon.Game;

public interface IPlayerLogics
{
    void OnAskForName();
    void OnAskForTurn();
    void OnAskToCut();
    void OnAskForContinue();
    void OnAskForInitialBank();
    void OnAskForDuel();
}