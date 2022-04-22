using System;
using agora_gaming_rtc;
using agora_rtm;
using AgoraIO.AccessToken;
using Chat.Agora;
using UnityEngine;


public class AgoraLogin : IChatLoginServices , IDisposable
{
    #region Private fields
    
    private IChatSystem m_chatSystem;
    //Voice chat
    private IRtcEngine m_rtcEngine;
    //
    
    private string m_strAppId, m_strRtmTokenKey, m_strAppCertificate;
    
    private uint m_iTs = 1111111;
    private uint m_iSalt = 1;
    
    private AgoraMessengerLogin m_agoraMessenger;
    private IChatConnectionEvents m_connectionEvents;
    private IChatDebugEvents m_debugEvents;
    private IChatMessageService m_messageService;
    #endregion
    
    #region Public fields
    #endregion
    
    public AgoraLogin(Action<bool> OnVivoxInitialized, string aAppId, string aAppCert, string aRtmTokenKey, IChatSystem aChatSystem)
    {
        if(string.IsNullOrEmpty(aAppId))
            Debug.LogError($"[AgoraLogin] App Id is empty");
        
        m_strAppId = aAppId;
        
        if(string.IsNullOrEmpty(aAppCert))
            Debug.LogError($"[AgoraLogin] App Certification is empty");
        m_strAppCertificate = aAppCert;
        
        m_strRtmTokenKey = aRtmTokenKey;
        
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
         m_rtcEngine.SetAudioProfile(AUDIO_PROFILE_TYPE.AUDIO_PROFILE_SPEECH_STANDARD,AUDIO_SCENARIO_TYPE.AUDIO_SCENARIO_CHATROOM_GAMING);
         
         if (m_rtcEngine == null)
         {
             OnVivoxInitialized.Invoke(false);
             return;
         }
         OnVivoxInitialized.Invoke(true);
         
         m_rtcEngine.OnLocalUserRegistered += OnLoginComplete;

         m_connectionEvents = new AgoraConnectionStatus(m_rtcEngine);
         // m_MessageService = new AgoraChatMessageService(m_rtcEngine);
         m_agoraMessenger = new AgoraMessengerLogin(this, m_strAppId, m_strAppCertificate, m_strRtmTokenKey);
         
         DependencyContainer.instance.RegisterToContainer<IChatLoginServices>(this);
    }

    private void OnLoginComplete(uint aUid, string aUsername)
    {
        
    }
    
    public void Login(string aDisplayName)
    {
        m_chatSystem.OnLoginComplete(m_rtcEngine.RegisterLocalUserAccount(m_strAppId, aDisplayName) >= 0);
        m_agoraMessenger.Login(aDisplayName);
    }

    public void Logout()
    {
        m_agoraMessenger.Logout();
    }

    public void CreateAndJoinChannel(string aTokenKey, string aChannelId, string aUsername, ChannelMediaOptions options)
    {
        Debug.Log($"[AgoraLogin][CreateAndJoinChannel] App Id: {m_strAppId} App Cert: {m_strAppCertificate} Channel Id: {aChannelId} Username: {aUsername}");
        Debug.Log($"[AgoraLogin][CreateAndJoinChannel] Channel Length: {aChannelId.Length} Username Length: {aUsername.Length}");
        //Generating token for voice chat here 
        AccessToken accessToken =
            new AccessToken(m_strAppId, m_strAppCertificate, aChannelId, "");
        accessToken.addPrivilege(Privileges.kJoinChannel,(uint)(Utils.getTimestamp()+90));
        accessToken.addPrivilege(Privileges.kPublishAudioStream,(uint)(Utils.getTimestamp()+90));
        accessToken.addPrivilege(Privileges.kPublishDataStream,(uint)(Utils.getTimestamp()+90));
        string strTokenKey = accessToken.build();
         // string strTokenKey = aTokenKey;
        
        Debug.Log($"[AgoraLogin] Voice Chat Token: {strTokenKey}");
        // m_rtcEngine.JoinChannelByKey(strTokenKey, aChannelId, "", (uint)UnityEngine.Random.Range(1,999));
        
        //Voice chat API calling 
        m_rtcEngine.JoinChannelWithUserAccount(strTokenKey, aChannelId, aUsername, options);
        
        //Messenger API calling 
        m_agoraMessenger.CreateAndJoinChannel("",aChannelId,"",null);
        
        Debug.Log($"[{GetType()}][CreateAndJoinChannel] Joining channel{aChannelId}");
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
        m_agoraMessenger.Dispose();
        m_agoraMessenger = null;
    }
}
