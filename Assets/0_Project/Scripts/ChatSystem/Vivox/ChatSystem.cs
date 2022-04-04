using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using VivoxUnity;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

namespace Chat.Vivox
{
    public class ChatSystem : MonoBehaviour , IChatSystem
    {
        
        #region Serialize fields

        [SerializeField] private InputField m_inputFieldNetworkId,m_inputFieldChannelName;
        [SerializeField] private Text m_textLoginStatus;
        [SerializeField] private RectTransform m_rectJoinNetworkUi, m_rectChatUi;
        [SerializeField] private RectTransform m_rectVivoxLogin, m_rectUsername;
        #endregion

        #region Private fields

        private IChatLoginService m_ChatLoginService;
        private IChatMessageService m_ChatMessageService;
        private IChatEventsService m_ChatEventsService;

        private bool m_bLoginSuccess = false, m_bCreatedChannel = false;
        private string m_strUserName = string.Empty;
        #endregion
        
        #region Public fields

        public bool ConnectionComplete { get; private set; }

        #endregion
        
        public void Inject(IChatEventsService aChatEventsService, IChatMessageService aChatMessageService)
        {
            m_ChatEventsService = aChatEventsService;
            m_ChatEventsService.RegisterOnUserConnectionChange(OnUserConnectStateChange);
            
            m_ChatMessageService = aChatMessageService;
            
            ConnectionComplete = true;
        }

        public void Login(string aUserName)
        {
            m_strUserName = aUserName;
            m_rectUsername.gameObject.SetActive(false);
            m_textLoginStatus.text = "Logging into Vivox as "+m_strUserName;
            Debug.Log($"<color=green> Username: {m_strUserName}</color>");
            m_ChatLoginService.Login(m_strUserName);
        }

        public void Logout()
        {
           m_ChatLoginService.Logout();
        }

        public void OnLoginComplete(bool aSuccess)
        {
            m_bLoginSuccess = aSuccess;
            if (aSuccess)
            {
                m_textLoginStatus.text = "Login complete";
                Debug.Log("[ChatSystem] Vivox login success");
                m_rectVivoxLogin.gameObject.SetActive(false);
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
                
                // StartCoroutine(VivoxUnityRun());
              
            }
            else
            {
                Debug.Log("[ChatSystem] Vivox channel not joined");
            }
        }
        
        // private IEnumerator VivoxUnityRun()
        // {
        //     while (VxClient.Instance.Started)
        //     {
        //         try
        //         {
        //             Client.RunOnce();               
        //         }
        //         catch (Exception e)
        //         {
        //             Debug.LogError("Error: " + e.Message); 
        //         }
        //         yield return new WaitForSeconds(0.01f);
        //     }
        // }

        private void OnDestroy()
        {
            if (m_ChatEventsService != null)
            {
                m_ChatEventsService.DeregisterOnUserConnectionChange(OnUserConnectStateChange);
            }
        }

        private void Start()
        {
#if PLATFORM_ANDROID
            if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
            {
                Permission.RequestUserPermission(Permission.Microphone);
            }
#endif
            m_rectVivoxLogin.gameObject.SetActive(true);
            DependencyContainer.instance.RegisterToContainer<IChatSystem>(this);
            m_textLoginStatus.text = "Initializing Vivox";
            m_ChatLoginService = new VivoxLogin((b =>
            {
                if (b)
                {
                    m_textLoginStatus.text = "Vivox initialized";
                    Debug.Log("[ChatSystem] Vivox initialization success");
                    m_rectUsername.gameObject.SetActive(true);
                }
                else
                {
                    Debug.LogError("[ChatSystem] Vivox initialization failed");
                }

            }), this);


        }
        

        public void CreateAndJoinChannel(string aChannelName)
        {
            if (!m_bLoginSuccess)
            {
                Debug.LogWarning("[ChatSystem] Login isn't complete yet");
                return;
            }
            
            m_ChatLoginService.CreateAndJoinChannel(aChannelName, ChannelType.NonPositional, true, true, true, null);
        }
        
        public void SendChatMessageToAll(string aMessage)
        {
            m_ChatMessageService.SendChatMessageToAll(aMessage,m_ChatLoginService.AccountId);
        }

        public void LeaveChannel()
        {
          m_ChatLoginService.LeaveChannel();
        }
        
        private void OnUserConnectStateChange(IChannelUserData aChannelUserData)
        {
            if(aChannelUserData.ParticipantJoined)
            {
                Debug.Log($"<color=green>[ChatSystem] Vivox user entered: {aChannelUserData.Username} </color>");
                if (aChannelUserData.Username == m_strUserName)
                {
                    m_rectJoinNetworkUi.gameObject.SetActive(false);
                    m_rectChatUi.gameObject.SetActive(true);
                }
            }
            else
            {
                Debug.Log($"<color=green>[ChatSystem] Vivox user exited: {aChannelUserData.Username} </color>");
                if (aChannelUserData.Username == m_strUserName)
                {
                    m_rectJoinNetworkUi.gameObject.SetActive(true);
                    m_rectChatUi.gameObject.SetActive(false);
                }
            }
        }
        
       
    }
}
