namespace OkoServer.Interface;

public interface IPlayer
{
    IAction GetAction();
    bool SendAction(IAction action);
}