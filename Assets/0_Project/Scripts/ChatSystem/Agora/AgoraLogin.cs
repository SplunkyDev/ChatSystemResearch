using System;
using agora_gaming_rtc;
using Chat.Agora;
using UnityEngine;


public class AgoraLogin : IChatLoginServices
{
    
    #region Serialized fields
    //This needs to be handled differently on production
    [SerializeField] private string m_strAppId;
    //This needs to be handled differently on production
    [SerializeField] private string m_strTokenKey;
    #endregion
    
    #region Private fields
    private IChatSystem m_chatSystem;
    private IRtcEngine m_rtcEngine;
    #endregion
    
    #region Public fields
    #endregion
    
    public AgoraLogin(Action<bool> OnVivoxInitialized, IChatSystem aChatSystem)
    {
        InitializeAgora(OnVivoxInitialized);
        m_chatSystem = aChatSystem;
    }

    private async void InitializeAgora(Action<bool> OnVivoxInitialized)
    {
         m_rtcEngine = IRtcEngine.GetEngine(m_strAppId);
         if (m_rtcEngine == null)
         {
             OnVivoxInitialized.Invoke(false);
             return;
         }
         
         
    }

    public void Login(string aDisplayName)
    {
        throw new System.NotImplementedException();
    }

    public void Logout()
    {
        throw new System.NotImplementedException();
    }

    public void CreateAndJoinChannel(string token, string channelId, string info, uint uid, ChannelMediaOptions options)
    {
        throw new System.NotImplementedException();
    }

    public void LeaveChannel()
    {
        throw new System.NotImplementedException();
    }
}
