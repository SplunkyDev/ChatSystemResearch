using BSS.Octane.Chat.Vivox;

public interface IChatSystem
{
    bool ConnectionComplete { get; }
    void Inject(IChatServiceEvents aChatServiceEvents, IChatServiceMessages aChatServiceMessages);
    void OnLoginComplete(bool aSuccess);

}
