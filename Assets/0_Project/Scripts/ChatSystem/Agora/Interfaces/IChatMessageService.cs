namespace Chat.Agora
{
    public interface IChatMessageService
    {
        void SendMessageToAll(string aMessage);
        
        //TODO: Look into Peer to Peer messaging
        // void SendMessageToSpecifUser();
        
        //On message received callback
        void RegisterOnChatMessageReceived(System.Action<IMessage> aEvent);
        void DeregisterOnChatMessageReceived(System.Action<IMessage> aEvent);
        
    }
}