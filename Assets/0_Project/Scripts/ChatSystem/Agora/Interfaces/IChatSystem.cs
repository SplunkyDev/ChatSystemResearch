namespace Chat.Agora
{
    public interface IChatSystem
    {
        bool ConnectionComplete { get; }
        void Inject(IChatConnectionEvents aConnectionEvents, IChatMessageServices aMessageServices);
        void Login(string aUser);
        void Logout();
        void OnLoginComplete(bool aSuccess);
        void OnChannelJoined(bool aSuccess);
        void CreateAndJoinChannel(string aChannelName);
        void SendChatMessageToAll(string aMessage);
        void LeaveChannel();
        
        //TODO: Implement specific player message
        // void SendChatMessageToPlayer(string aMessage);
    }
}
