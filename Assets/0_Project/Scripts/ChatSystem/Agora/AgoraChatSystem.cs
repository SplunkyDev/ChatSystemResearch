using System.Collections;
using System.Collections.Generic;
using agora_gaming_rtc;
using Chat.Agora;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

namespace Chat.Agora
{
    public class AgoraChatSystem : MonoBehaviour , IChatSystem
    {
        #region Serialize fields
        //This needs to be handled differently on production
        [SerializeField] private string m_strAppId;
        //This needs to be handled differently on production
        [SerializeField] private string m_strTokenKey;
        [SerializeField] private InputField m_inputFieldUsername,m_inputFieldChannelName;
        [SerializeField] private Text m_textLoginStatus;
        [SerializeField] private RectTransform m_rectJoinNetworkUi, m_rectChatUi;
        [SerializeField] private RectTransform m_rectLogin, m_rectUsername;
        #endregion

        #region Private fields

        private IChatLoginServices m_chatLoginServices;
        private IChatConnectionEvents m_connectionEvents;
        private IChatMessageService m_messageService;

        private bool m_bLoginSuccess = false, m_bCreatedChannel = false;
        private string m_strUserName = string.Empty;
        #endregion
        
        #region Public fields

        public bool ConnectionComplete { get; private set; }

        #endregion
        
        
        public void Inject(IChatConnectionEvents aConnectionEvents, IChatMessageService aMessageService)
        {
            m_connectionEvents = aConnectionEvents;
            m_connectionEvents.RegisterOnChannelJoinOrLeft(OnUserConnectStateChange);
            m_messageService = aMessageService;
            ConnectionComplete = true;
            
        }
        

        public void Login(string aUserName)
        {
            m_strUserName = aUserName;
            m_rectUsername.gameObject.SetActive(false);
            m_textLoginStatus.text = "Logging into Agora as "+m_strUserName;
            Debug.Log($"<color=green> Username: {m_strUserName}</color>");
            m_chatLoginServices.Login(m_strUserName);
        }

        public void Logout()
        {
            m_chatLoginServices.Logout();
        }

        public void OnLoginComplete(bool aSuccess)
        {
            m_bLoginSuccess = aSuccess;
            if (aSuccess)
            {
                m_textLoginStatus.text = "Login complete";
                Debug.Log("[ChatSystem] Vivox login success");
                m_rectLogin.gameObject.SetActive(false);
                m_rectJoinNetworkUi.gameObject.SetActive(true);
                
            }
            else
            {
                Debug.Log("[ChatSystem] Vivox login failed");
                m_textLoginStatus.text = "Login to vivox failed. Restart application.";
            }
        }

        public void OnChannelJoined(bool aSuccess)
        {
            if (aSuccess)
            {
                Debug.Log("[ChatSystem] Vivox channel joined");
                
                if(!m_bCreatedChannel)
                {
                    m_rectJoinNetworkUi.gameObject.SetActive(false);
                    m_rectChatUi.gameObject.SetActive(true);
                }

            }
            else
            {
                Debug.Log("[ChatSystem] Vivox channel not joined");
            }
        }

        private void OnDestroy()
        {
            m_connectionEvents?.DeregisterOnChannelJoinOrLeft(OnUserConnectStateChange);
        }

        private void Start()
        {
#if PLATFORM_ANDROID
            if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
            {
                Permission.RequestUserPermission(Permission.Microphone);
            }
#endif
            m_rectLogin.gameObject.SetActive(true);
            DependencyContainer.instance.RegisterToContainer<IChatSystem>(this);
            m_textLoginStatus.text = "Initializing Agora";
            m_chatLoginServices = new AgoraLogin((b =>
            {
                if (b)
                {
                    m_textLoginStatus.text = "Agora initialized";
                    Debug.Log("[ChatSystem] Agora initialization success");
                    m_rectUsername.gameObject.SetActive(true);
                }
                else
                {
                    Debug.LogError("[ChatSystem] Agora initialization failed");
                }
            
            }),m_strAppId, this);


        }
        

        public void CreateAndJoinChannel(string aChannelName)
        {
            if (!m_bLoginSuccess)
            {
                Debug.LogWarning("[ChatSystem] Login isn't complete yet");
                return;
            }

            m_chatLoginServices.CreateAndJoinChannel(m_strTokenKey,aChannelName,m_strUserName,new ChannelMediaOptions(true,false,true,false));
        }
        
        public void SendChatMessageToAll(string aMessage)
        {
           m_messageService.SendMessageToAll(aMessage);
        }

        public void LeaveChannel()
        {
            m_chatLoginServices.LeaveChannel();
        }
        
        private void OnUserConnectStateChange(IChannelUserStatus aChannelUserStatus)
        {
            if(aChannelUserStatus.ParticipantJoined)
            {
                Debug.Log($"<color=green>[ChatSystem] user entered: {aChannelUserStatus.Username} </color>");
                if (aChannelUserStatus.Username == m_strUserName)
                {
                    m_rectJoinNetworkUi.gameObject.SetActive(false);
                    m_rectChatUi.gameObject.SetActive(true);
                }
            }
            else
            {
                Debug.Log($"<color=green>[ChatSystem] user exited: {aChannelUserStatus.Username} </color>");
                if (aChannelUserStatus.Username == m_strUserName)
                {
                    m_rectJoinNetworkUi.gameObject.SetActive(true);
                    m_rectChatUi.gameObject.SetActive(false);
                }
            }
        }
        
       
    }
}

