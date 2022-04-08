using System;
using agora_gaming_rtc;
using agora_rtm;
using Chat.Agora;



public class AgoraLogin : IChatLoginServices , IDisposable
{
    #region Private fields
    
    private IChatSystem m_chatSystem;
    //Voice chat
    private IRtcEngine m_rtcEngine;
    //
    
    private string m_strAppId, m_strTokenKey;

    private IChatConnectionEvents m_connectionEvents;
    private IChatDebugEvents m_debugEvents;
    private IChatMessageService _mMessageService;
    #endregion
    
    #region Public fields
    #endregion
    
    public AgoraLogin(Action<bool> OnVivoxInitialized, string aAppId, string aTokenKey, IChatSystem aChatSystem)
    {
        m_strAppId = aAppId;
        m_strTokenKey = aTokenKey;
        m_chatSystem = aChatSystem;
        InitializeAgora(OnVivoxInitialized);
    }

    private async void InitializeAgora(Action<bool> OnVivoxInitialized)
    {
         m_rtcEngine = IRtcEngine.GetEngine(m_strAppId);
         if (m_rtcEngine == null)
         {
             OnVivoxInitialized.Invoke(false);
             return;
         }
         OnVivoxInitialized.Invoke(true);
         
         m_rtcEngine.OnLocalUserRegistered += OnLoginComplete;

         m_connectionEvents = new AgoraConnectionStatus(m_rtcEngine);
         _mMessageService = new AgoraMessengerLogin(m_rtcEngine);
         
         DependencyContainer.instance.RegisterToContainer<IChatLoginServices>(this);
         DependencyContainer.instance.RegisterToContainer<IChatConnectionEvents>(m_connectionEvents);
         DependencyContainer.instance.RegisterToContainer<IChatMessageService>(_mMessageService);
         
         m_chatSystem.Inject(m_connectionEvents,_mMessageService);
         
         
    }

    private void OnLoginComplete(uint aUid, string aUsername)
    {
        
    }
    
    public void Login(string aDisplayName)
    {
        m_chatSystem.OnLoginComplete(m_rtcEngine.RegisterLocalUserAccount(m_strAppId, aDisplayName) >= 0);
    }

    public void Logout()
    {

    }

    public void CreateAndJoinChannel(string aTokenKey, string aChannelId, string aUsername, ChannelMediaOptions options)
    {
        m_rtcEngine.JoinChannelWithUserAccount(aTokenKey, aChannelId, aUsername, options);
    }

    public void LeaveChannel()
    {
        m_rtcEngine.LeaveChannel();
    }

    public void Dispose()
    {
        if(m_rtcEngine != null)
        {
            m_rtcEngine.OnLocalUserRegistered -= OnLoginComplete;
            m_rtcEngine.LeaveChannel();
        }
        IRtcEngine.Destroy();
    }
}
