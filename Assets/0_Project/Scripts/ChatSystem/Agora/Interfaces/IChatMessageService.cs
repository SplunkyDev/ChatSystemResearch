namespace Chat.Agora
{
    public interface IChatMessageService
    {
        void SendMessageToAll(string aMessage);
        void SendMessageToSpecifUser(string aUsername, string aMessage);
        
        //On message received callback
        void RegisterOnChatMessageReceived(System.Action<IMessage> aEvent);
        void DeregisterOnChatMessageReceived(System.Action<IMessage> aEvent);
        
    }
}