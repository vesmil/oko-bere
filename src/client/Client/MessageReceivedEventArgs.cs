using OkoCommon.Communication;

namespace OkoClient.Client;

/// <summary>
///     Recieved message wrapper for event handling.
/// </summary>
public class MessageReceivedEventArgs : EventArgs
{
    public MessageReceivedEventArgs(object? data, NotifEnum type)
    {
        Data = data;
        Type = type;
    }

    public object? Data { get; }
    public NotifEnum Type { get; }
}