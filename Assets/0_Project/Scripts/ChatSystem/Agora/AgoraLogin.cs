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

    private AgoraMessengerLogin m_agoraMessenger;
    private IChatConnectionEvents m_connectionEvents;
    private IChatDebugEvents m_debugEvents;
    private IChatMessageService m_messageService;
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

    public void Inject(IChatMessageService aMessageService)
    {
        m_messageService = aMessageService;
        m_chatSystem.Inject(m_connectionEvents,m_messageService);
        DependencyContainer.instance.RegisterToContainer<IChatConnectionEvents>(m_connectionEvents);
        DependencyContainer.instance.RegisterToContainer<IChatMessageService>(m_messageService);
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
         // m_MessageService = new AgoraChatMessageService(m_rtcEngine);
         m_agoraMessenger = new AgoraMessengerLogin(this, m_strAppId, m_strTokenKey);
         
         DependencyContainer.instance.RegisterToContainer<IChatLoginServices>(this);
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
        m_agoraMessenger.Dispose();
        m_agoraMessenger = null;
        if(m_rtcEngine != null)
        {
            m_rtcEngine.OnLocalUserRegistered -= OnLoginComplete;
            m_rtcEngine.LeaveChannel();
        }
        IRtcEngine.Destroy();
    }
}
