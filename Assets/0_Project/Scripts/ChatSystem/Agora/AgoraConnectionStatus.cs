using System;
using agora_gaming_rtc;
using Chat.Agora;
using UnityEngine;

public class AgoraConnectionStatus : IChatConnectionEvents, IDisposable
{
    #region Private fields
    private System.Action<IChannelUserStatus> OnChannelJoinOrLeft;
    private System.Action<IChannelConnectionStatus> OnConnectionStatus;
    private IRtcEngine m_rtcEngine;
    #endregion
    
    #region Public fields
    #endregion

    public AgoraConnectionStatus(IRtcEngine aRtcEngine)
    {
        m_rtcEngine = aRtcEngine;

        //Join/left callbacks
        m_rtcEngine.OnUserJoined += OnRemoteUserJoined;
        m_rtcEngine.OnUserOffline += OnRemoteUserLeft;
        
        //Connection callbacks
        m_rtcEngine.OnConnectionStateChanged += OnConnectionChange;
    }

    private void OnRemoteUserJoined(uint uid, int elapsed)
    {
        var userInfo = m_rtcEngine.GetUserInfoByUid(uid);
        ChannelUserStatus channelUserStatus = new ChannelUserStatus(userInfo.userAccount,uid,true);
        OnChannelJoinOrLeft?.Invoke(channelUserStatus);
        
        Debug.Log($"[AgoraConnectionStatus] Remote player connected user id: {uid} user name: {userInfo.userAccount}");
    }

    private void OnRemoteUserLeft(uint uid, USER_OFFLINE_REASON reason)
    {
        var userInfo = m_rtcEngine.GetUserInfoByUid(uid);
        ChannelUserStatus channelUserStatus = new ChannelUserStatus(userInfo.userAccount,uid,false);
        OnChannelJoinOrLeft?.Invoke(channelUserStatus);
        
        Debug.Log($"[AgoraConnectionStatus] Remote player disconnected user id: {uid} user name: {userInfo.userAccount}");
    }
    
    private void OnConnectionChange(CONNECTION_STATE_TYPE state, CONNECTION_CHANGED_REASON_TYPE reason)
    {
        ChannelConnectionStatus channelConnectionStatus = new ChannelConnectionStatus(state, reason);
        OnConnectionStatus?.Invoke(channelConnectionStatus);
    }
    
    public void RegisterOnChannelJoinOrLeft(Action<IChannelUserStatus> aEvent)
    {
        OnChannelJoinOrLeft += aEvent;
    }

    public void DeregisterOnChannelJoinOrLeft(Action<IChannelUserStatus> aEvent)
    {
        OnChannelJoinOrLeft -= aEvent;
    }

    public void RegisterOnConnectionStatus(Action<IChannelConnectionStatus> aEvent)
    {
        OnConnectionStatus += aEvent;
    }

    public void DeregisterOnConnectionStatus(Action<IChannelConnectionStatus> aEvent)
    {
        OnConnectionStatus -= aEvent;
    }

    public void Dispose()
    {
        if(m_rtcEngine != null)
        {
            //Join/left callbacks
            m_rtcEngine.OnUserJoined -= OnRemoteUserJoined;
            m_rtcEngine.OnUserOffline -= OnRemoteUserLeft;

            //Connection callbacks
            m_rtcEngine.OnConnectionStateChanged -= OnConnectionChange;
        }
    }
}
