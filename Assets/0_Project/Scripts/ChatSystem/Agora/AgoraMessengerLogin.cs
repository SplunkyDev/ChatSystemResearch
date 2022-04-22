using System;
using System.Collections;
using System.Collections.Generic;
using agora_gaming_rtc;
using agora_rtm;
using AgoraIO.Media;
using Chat.Agora;
using UnityEngine;

public class AgoraMessengerLogin : IChatLoginServices, IDisposable
{
    #region  Private fields

    private string m_strAppId, m_strTokenKey, m_strUsername, m_strChannelName, m_strAppCertificate;
    //Real time messaging
    private RtmClient m_rtmClient;
    private RtmChannel m_rtmChannel;
    //
    
    private RtmClientEventHandler m_clientEventHandler;
    private RtmChannelEventHandler m_channelEventHandler;
    private RtmCallEventHandler m_callEventHandler;
    
    private AgoraLogin m_agoraLogin;
    private AgoraMessengerService m_agoraMessengerService;

    private List<string> m_lstJoinedMembers = new List<string>();
    private long m_lRequestId = 0;
    #endregion

    public AgoraMessengerLogin(AgoraLogin aAgoraLogin, string aAppId, string aAppCert, string aTokenKey)
    {
        if(string.IsNullOrEmpty(aAppId))
            Debug.LogError($"[AgoraMessengerLogin] App Id is empty");
        
        m_strAppId = aAppId;
        
        if(string.IsNullOrEmpty(aAppCert))
            Debug.LogError($"[AgoraMessengerLogin] App Certification is empty");
        m_strAppCertificate = aAppCert;
        
        m_strTokenKey = aTokenKey;
        
        m_agoraLogin = aAgoraLogin;
        
        m_clientEventHandler = new RtmClientEventHandler();
        m_channelEventHandler = new RtmChannelEventHandler();
        // m_callEventHandler = new RtmCallEventHandler();
        m_clientEventHandler.OnQueryPeersOnlineStatusResult += OnQueryPeersOnlineStatusResultHandler;
        m_clientEventHandler.OnLoginSuccess += OnClientLoginSuccessHandler;
        m_clientEventHandler.OnLoginFailure += OnClientLoginFailureHandler;
        m_clientEventHandler.OnMessageReceivedFromPeer += OnMessageReceivedFromPeerHandler;
        m_clientEventHandler.OnPeersOnlineStatusChanged += OnPeersOnlineStatusChangedHandler;
        
        m_channelEventHandler.OnJoinSuccess += OnJoinSuccessHandler;
        m_channelEventHandler.OnJoinFailure += OnJoinFailureHandler;
        m_channelEventHandler.OnLeave += OnLeaveHandler;
        m_channelEventHandler.OnMessageReceived += OnChannelMessageReceivedHandler;
        m_channelEventHandler.OnMemberJoined += OnMemberJoinedHandler;
        m_channelEventHandler.OnMemberLeft += OnMemberLeftHandler;
        
        m_rtmClient = new RtmClient(aAppId,m_clientEventHandler);

      
        
        m_agoraMessengerService =
            new AgoraMessengerService(m_rtmClient, m_clientEventHandler);
        m_agoraLogin.Inject(m_agoraMessengerService);
    }

    public void Login(string aDisplayName)
    {
        if (string.IsNullOrEmpty(aDisplayName))
        {
            Debug.LogError($"[{GetType()}][Login] username is empty. Cannot login");
            return;
        }
        
        m_strUsername = aDisplayName;
        
        //Generate token for real time messaging using username here 
        AccessToken accessToken =
            new AccessToken(m_strAppId, m_strAppCertificate, aDisplayName, "");
        accessToken.addPrivilege(Privileges.kRtmLogin,(uint)(Utils.getTimestamp()+90));
        m_strTokenKey = accessToken.build();
        Debug.Log($"<color=green>[AgoraMessengerLogin] Real-time messaging Token: {m_strTokenKey}</color>");
        if (string.IsNullOrEmpty(m_strTokenKey))
        {
            Debug.LogError($"[{GetType()}][Login] Token is empty. Cannot login");
            return;
        }
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
        
        m_agoraMessengerService.Inject(m_rtmChannel,m_channelEventHandler);
        m_rtmChannel.Join();
        
        Debug.Log($"[{GetType()}][CreateAndJoinChannel] Joining messenger channel {aChannelId}");
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
        Debug.Log($"[{GetType()}][OnClientLoginSuccessHandler] {msg}");
        // messageDisplay.AddTextToDisplay(msg, Message.MessageType.Info);
    }
    
    // Callback when failed to log in
    void OnClientLoginFailureHandler(int id, LOGIN_ERR_CODE errorCode)
    {
        string msg = "client login unsuccessful! id = " + id + " errorCode = " + errorCode;
        Debug.Log($"[{GetType()}][OnClientLoginFailureHandler] {msg}");
        // messageDisplay.AddTextToDisplay(msg, Message.MessageType.Error);
    }
    
    // Callback when receiving a peer-to-peer message
    void OnMessageReceivedFromPeerHandler(int id, string peerId, TextMessage message)
    {
        Debug.Log("client OnMessageReceivedFromPeer id = " + id + ", from user:" + peerId + " text:" + message.GetText());
        // messageDisplay.AddTextToDisplay(peerId + ": " + message.GetText(), Message.MessageType.PeerMessage);
    }

    void OnPeersOnlineStatusChangedHandler(int aId, PeerOnlineStatus[] aPeerOnlineStatusArray, int aPeerCount)
    {
        Debug.Log($"Peer count: {aPeerCount} ");
        for (int i = 0; i < aPeerCount; i++)
        {
            Debug.Log($" Peer Id: {aPeerOnlineStatusArray[i].peerId} Peer status: {aPeerOnlineStatusArray[i].onlineState.ToString()} ");
        }
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
    
    // Callback when a remote member joins the channel
    void OnMemberJoinedHandler(int id, RtmChannelMember member)
    {
        string msg = "channel OnMemberJoinedHandler member ID=" + member.GetUserId() + " channelId = " + member.GetChannelId();
        Debug.Log(msg);

        if (!m_lstJoinedMembers.Contains(member.GetUserId()))
        {
            m_lstJoinedMembers.Add(member.GetUserId());
            m_lRequestId++;
            m_rtmClient.SubscribePeersOnlineStatus(m_lstJoinedMembers.ToArray(), m_lRequestId);
        }
    }
    
    // Callback when a remote member leaves the channel
    void OnMemberLeftHandler(int id, RtmChannelMember member)
    {
        string msg = "channel OnMemberLeftHandler member ID=" + member.GetUserId() + " channelId = " + member.GetChannelId();
        Debug.Log(msg);
    }
    #endregion

    public void Dispose()
    {

        if (m_clientEventHandler != null)
        {
            m_clientEventHandler.OnQueryPeersOnlineStatusResult -= OnQueryPeersOnlineStatusResultHandler;
            m_clientEventHandler.OnLoginSuccess -= OnClientLoginSuccessHandler;
            m_clientEventHandler.OnLoginFailure -= OnClientLoginFailureHandler;
            m_clientEventHandler.OnMessageReceivedFromPeer -= OnMessageReceivedFromPeerHandler;
            m_clientEventHandler.OnPeersOnlineStatusChanged += OnPeersOnlineStatusChangedHandler;
        }

        if (m_channelEventHandler != null)
        {
            m_channelEventHandler.OnJoinSuccess -= OnJoinSuccessHandler;
            m_channelEventHandler.OnJoinFailure -= OnJoinFailureHandler;
            m_channelEventHandler.OnLeave -= OnLeaveHandler;
            m_channelEventHandler.OnMessageReceived -= OnChannelMessageReceivedHandler;
            m_channelEventHandler.OnMemberJoined -= OnMemberJoinedHandler;
            m_channelEventHandler.OnMemberLeft -= OnMemberLeftHandler;
        }
        
        if(m_rtmClient != null)
        {
            m_rtmClient.UnsubscribePeersOnlineStatus(m_lstJoinedMembers.ToArray(), ++m_lRequestId);
            m_rtmClient.Dispose();
            m_rtmClient = null;
        }

        if (m_rtmChannel != null)
        {
            m_rtmChannel.Dispose();
            m_rtmChannel = null;
        }
    }
}
