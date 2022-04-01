using Chat.Vivox;

namespace Chat
{
    public interface IChatSystem
    {
        bool ConnectionComplete { get; }
        void Inject(IChatEventsService aChatEventsService, IChatMessageService aChatMessageService);
        void OnLoginComplete(bool aSuccess);
        void OnChannelJoined(bool aSuccess);
        void CreateAndJoinChannel(string aChannelName);
        void SendChatMessageToAll(string aMessage);
        
        //TODO: Implement specific player message
        // void SendChatMessageToPlayer(string aMessage);

    }
}
