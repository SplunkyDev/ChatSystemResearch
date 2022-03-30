using BSS.Octane.Chat.Vivox;

namespace BSS.Octane.Chat
{
    public interface IChatSystem
    {
        bool ConnectionComplete { get; }
        void Inject(IChatServiceEvents aChatServiceEvents, IChatServiceMessages aChatServiceMessages);
        void OnLoginComplete(bool aSuccess);
        void OnChannelJoined(bool aSuccess);
        void CreateAndJoinChannel(string aChannelName);
        void JoinChannel(string aChannelName);
        void SendChatMessageToAll(string aMessage);
        
        //TODO: Implement specific player message
        // void SendChatMessageToPlayer(string aMessage);

    }
}
