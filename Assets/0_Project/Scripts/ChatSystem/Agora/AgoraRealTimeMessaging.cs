using System.Collections;
using agora_gaming_rtc;
using agora_rtm;
using Chat.Agora;
using UnityEngine;

public class AgoraRealTimeMessaging : IChatLoginServices
{
    #region  Private fields

    private string m_strAppId, m_strTokenKey, m_strUsername, m_strChannelName;
    
    //Real time messaging
    private RtmClient m_rtmClient;
    private RtmChannel m_rtmChannel;
    //
    
    private RtmClientEventHandler m_clientEventHandler;
    private RtmChannelEventHandler m_channelEventHandler;
    private RtmCallEventHandler m_callEventHandler;
    
    private AgoraLogin m_agoraLogin;
    #endregion

    public AgoraRealTimeMessaging(AgoraLogin aAgoraLogin, string aAppId, string aTokenKey)
    {
        m_strAppId = aAppId;
        m_strTokenKey = aTokenKey;
        
        m_agoraLogin = aAgoraLogin;
        
        m_clientEventHandler = new RtmClientEventHandler();
        m_channelEventHandler = new RtmChannelEventHandler();
        // m_callEventHandler = new RtmCallEventHandler();
        
        m_rtmClient = new RtmClient(aAppId,m_clientEventHandler);
        
        m_clientEventHandler.OnQueryPeersOnlineStatusResult = OnQueryPeersOnlineStatusResultHandler;
        m_clientEventHandler.OnLoginSuccess = OnClientLoginSuccessHandler;
        m_clientEventHandler.OnLoginFailure = OnClientLoginFailureHandler;
        m_clientEventHandler.OnMessageReceivedFromPeer = OnMessageReceivedFromPeerHandler;
    }

    public void Login(string aDisplayName)
    {
        if (string.IsNullOrEmpty(aDisplayName))
        {
            Debug.LogError($"[{GetType()}][Login] username is empty. Cannot login");
            return;
        }

        if (string.IsNullOrEmpty(m_strTokenKey))
        {
            Debug.LogError($"[{GetType()}][Login] Token is empty. Cannot login");
            return;
        }

        m_strUsername = aDisplayName;
        m_rtmClient.Login(m_strTokenKey, m_strUsername);
    }

    public void Logout()
    {
        m_rtmClient.Logout();
    }

    public void CreateAndJoinChannel(string aTokenKey, string aChannelId, string aUsername, ChannelMediaOptions options)
    {
        m_strChannelName = aChannelId;
        m_rtmChannel = m_rtmClient.CreateChannel(aChannelId, m_channelEventHandler);
        
        m_channelEventHandler.OnJoinSuccess = OnJoinSuccessHandler;
        m_channelEventHandler.OnJoinFailure = OnJoinFailureHandler;
        m_channelEventHandler.OnLeave = OnLeaveHandler;
        m_channelEventHandler.OnMessageReceived = OnChannelMessageReceivedHandler;
        
        m_rtmChannel.Join();
    }

    public void LeaveChannel()
    {
        m_rtmChannel.Leave();
    }
    
    #region Client Events 
    private void OnQueryPeersOnlineStatusResultHandler(int id, long requestId, PeerOnlineStatus[] peersStatus, int peerCount, QUERY_PEERS_ONLINE_STATUS_ERR errorCode)
    {
        if (peersStatus.Length > 0)
        {
            Debug.Log("OnQueryPeersOnlineStatusResultHandler requestId = " + requestId +
                      " peersStatus: peerId=" + peersStatus[0].peerId +
                      " online=" + peersStatus[0].isOnline +
                      " onlinestate=" + peersStatus[0].onlineState);
            // messageDisplay.AddTextToDisplay("User " + peersStatus[0].peerId + " online status = " + peersStatus[0].onlineState, Message.MessageType.Info);
        }
    }
    
    // Callback when log in successfully
    void OnClientLoginSuccessHandler(int id)
    {
        string msg = "client login successful! id = " + id;
        Debug.Log(msg);
        // messageDisplay.AddTextToDisplay(msg, Message.MessageType.Info);
    }
    
    // Callback when failed to log in
    void OnClientLoginFailureHandler(int id, LOGIN_ERR_CODE errorCode)
    {
        string msg = "client login unsuccessful! id = " + id + " errorCode = " + errorCode;
        Debug.Log(msg);
        // messageDisplay.AddTextToDisplay(msg, Message.MessageType.Error);
    }
    
    // Callback when receiving a peer-to-peer message
    void OnMessageReceivedFromPeerHandler(int id, string peerId, TextMessage message)
    {
        Debug.Log("client OnMessageReceivedFromPeer id = " + id + ", from user:" + peerId + " text:" + message.GetText());
        // messageDisplay.AddTextToDisplay(peerId + ": " + message.GetText(), Message.MessageType.PeerMessage);
    }
    #endregion
    
    #region Channel Events
    // Callback when joining a channel successfully
    void OnJoinSuccessHandler(int id)
    {
        string msg = "channel:" + m_strChannelName + " OnJoinSuccess id = " + id;
        Debug.Log(msg);
        // messageDisplay.AddTextToDisplay(msg, Message.MessageType.Info);

    }
    // Callback when failed to join a channel
    void OnJoinFailureHandler(int id, JOIN_CHANNEL_ERR errorCode)
    {
        string msg = "channel OnJoinFailure  id = " + id + " errorCode = " + errorCode;
        Debug.Log(msg);
        // messageDisplay.AddTextToDisplay(msg, Message.MessageType.Error);
    }
    
    // Callback when leave the channel
    void OnLeaveHandler(int id, LEAVE_CHANNEL_ERR errorCode)
    {
        string msg = "client onleave id = " + id + " errorCode = " + errorCode;
        Debug.Log(msg);
        // messageDisplay.AddTextToDisplay(msg, Message.MessageType.Info);
    }
    // Callback when receiving a channel message
    void OnChannelMessageReceivedHandler(int id, string userId, TextMessage message)
    {
        Debug.Log("client OnChannelMessageReceived id = " + id + ", from user:" + userId + " text:" + message.GetText());
        // messageDisplay.AddTextToDisplay(userId + ": " + message.GetText(), Message.MessageType.ChannelMessage);
    }
    
    #endregion
}
