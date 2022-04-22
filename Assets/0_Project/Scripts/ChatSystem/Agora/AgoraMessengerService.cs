using System;
using agora_rtm;
using Chat;
using UnityEngine;
using Chat.Agora;

public class AgoraMessengerService :  IChatMessageService, IDisposable
{
    #region Private fields
    private System.Action<Chat.IMessage> OnMessageReceived;
    private RtmClient m_rtmClient;
    private RtmChannel m_rtmChannel;
    
    
    private RtmClientEventHandler m_clientEventHandler;
    private RtmChannelEventHandler m_channelEventHandler;
    
    #endregion
    
    #region Public fields
    #endregion
    
    public AgoraMessengerService(RtmClient aRtmClient, RtmClientEventHandler aClientEventHandler )
    {
        m_rtmClient = aRtmClient;
        
        m_clientEventHandler = aClientEventHandler;
        m_clientEventHandler.OnMessageReceivedFromPeer += OnMessageReceivedFromPeerHandler;
      
    }

    public void Inject(RtmChannel aRtmChannel, RtmChannelEventHandler aChannelEventHandler)
    {
        m_rtmChannel = aRtmChannel;
        m_channelEventHandler = aChannelEventHandler;
        m_channelEventHandler.OnMessageReceived += OnChannelMessageReceivedHandler;
    }
    
    // Callback when receiving a peer-to-peer message
    private void OnMessageReceivedFromPeerHandler(int id, string peerId, TextMessage message)
    {
        Debug.Log($"[{GetType().Name}]client OnMessageReceivedFromPeer id: {id} , from user:  {peerId}  text: {message.GetText()}");
        ChatMessage chatMessage = new ChatMessage(id.ToString(), peerId,  message.GetText());
        OnMessageReceived?.Invoke(chatMessage);
    }
    
    // Callback when receiving a channel message
    void OnChannelMessageReceivedHandler(int id, string userId, TextMessage message)
    {
        Debug.Log($"[{GetType().Name}]client OnChannelMessageReceived id: {id} , from user:  {userId}  text: {message.GetText()}");
        ChatMessage chatMessage = new ChatMessage(id.ToString(), userId,  message.GetText());
        OnMessageReceived?.Invoke(chatMessage);
    }
    
    public void SendMessageToAll(string aMessage)
    {
        if (m_rtmChannel == null)
        {
            Debug.LogError($"[{GetType()}][SendMessageToAll] Channel reference is null,cannot send message");
            return;
        }
        Debug.Log($"<color=green>[{GetType().Name}] Send message to all users in channel: {aMessage}</color>");
        m_rtmChannel.SendMessage(m_rtmClient.CreateMessage(aMessage));
    }

    public void SendMessageToSpecifUser(string aUsername, string aMessage)
    {
        if (m_rtmClient == null)
        {
            Debug.LogError($"[{GetType()}][SendMessageToSpecifUser] Client reference is null,cannot send peer to peer message");
            return;
        }
        Debug.Log($"<color=green>[{GetType().Name}] Send message to {aUsername} user in channel: {aMessage}</color>");
        m_rtmClient.SendMessageToPeer(aUsername, m_rtmClient.CreateMessage(aMessage));
    }

    public void RegisterOnChatMessageReceived(Action<Chat.IMessage> aEvent)
    {
        OnMessageReceived += aEvent;
    }

    public void DeregisterOnChatMessageReceived(Action<Chat.IMessage> aEvent)
    {
        OnMessageReceived -= aEvent;
    }

    public void Dispose()
    {
        if(m_clientEventHandler != null)
        {
            m_clientEventHandler.OnMessageReceivedFromPeer += OnMessageReceivedFromPeerHandler;
        }
       if(m_channelEventHandler != null)
       {
           m_channelEventHandler.OnMessageReceived += OnChannelMessageReceivedHandler;
       }
    }
}
