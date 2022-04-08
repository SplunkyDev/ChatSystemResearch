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
    
    public AgoraMessengerService(RtmClient aRtmClient,RtmChannel aRtmChannel, RtmClientEventHandler aClientEventHandler, RtmChannelEventHandler aChannelEventHandler)
    {
        m_rtmClient = aRtmClient;
        m_rtmChannel = aRtmChannel;
        m_clientEventHandler = aClientEventHandler;
        m_clientEventHandler.OnMessageReceivedFromPeer += OnMessageReceivedFromPeerHandler;
        m_channelEventHandler = aChannelEventHandler;
        m_channelEventHandler.OnMessageReceived += OnChannelMessageReceivedHandler;
    }

    // Callback when receiving a peer-to-peer message
    private void OnMessageReceivedFromPeerHandler(int id, string peerId, TextMessage message)
    {
        Debug.Log("client OnMessageReceivedFromPeer id = " + id + ", from user:" + peerId + " text:" + message.GetText());
        ChatMessage chatMessage = new ChatMessage(id.ToString(), peerId,  message.GetText());
        OnMessageReceived?.Invoke(chatMessage);
    }
    
    // Callback when receiving a channel message
    void OnChannelMessageReceivedHandler(int id, string userId, TextMessage message)
    {
        Debug.Log("client OnChannelMessageReceived id = " + id + ", from user:" + userId + " text:" + message.GetText());
        ChatMessage chatMessage = new ChatMessage(id.ToString(), userId,  message.GetText());
        OnMessageReceived?.Invoke(chatMessage);
    }
    
    public void SendMessageToAll(string aMessage)
    {
        m_rtmChannel.SendMessage(m_rtmClient.CreateMessage(aMessage));
    }

    public void SendMessageToSpecifUser(string aUsername, string aMessage)
    {
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
