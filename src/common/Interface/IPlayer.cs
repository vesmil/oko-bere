namespace OkoCommon.Interface;

public interface IPlayer
{
    IAction GetAction();
    bool SendAction(IAction action);
}