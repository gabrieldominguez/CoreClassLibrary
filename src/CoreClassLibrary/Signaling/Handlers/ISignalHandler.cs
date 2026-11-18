namespace CoreClassLibrary.Signaling.Handlers;

public interface ISignalHandler
    {
    void EventHandler(object? sender, EventArgs e);
    }
