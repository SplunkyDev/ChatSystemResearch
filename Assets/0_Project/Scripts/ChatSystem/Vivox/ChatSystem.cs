using System;
using UnityEngine;
using UnityEngine.UI;
using VivoxUnity;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

namespace BSS.Octane.Chat.Vivox
{
    public class ChatSystem : MonoBehaviour , IChatSystem
    {
        #region Serialize fields

        [SerializeField] private InputField m_inputFieldNetworkId,m_inputFieldChannelName;
        [SerializeField] private Text m_textLoginStatus;
        [SerializeField] private RectTransform m_rectJoinNetworkUi, m_rectChatUi, m_rectPlayFabLogin;
        [SerializeField] private string m_strUserName;
        #endregion

        #region Private fields

        private IChatServiceLogin m_chatServiceLogin;
        private IChatServiceMessages m_chatServiceMessages;
        private IChatServiceEvents m_chatServiceEvents;

        private bool m_bLoginSuccess = false, m_bCreatedChannel = false;

        #endregion
        
        #region Public fields

        public bool ConnectionComplete { get; private set; }

        #endregion
        
        public void Inject(IChatServiceEvents aChatServiceEvents, IChatServiceMessages aChatServiceMessages)
        {
            m_chatServiceEvents = aChatServiceEvents;
            m_chatServiceEvents.RegisterOnUserConnectionChange(OnUserConnectStateChange);
            
            m_chatServiceMessages = aChatServiceMessages;
            
            ConnectionComplete = true;
        }

        public void OnLoginComplete(bool aSuccess)
        {
            m_bLoginSuccess = aSuccess;
            if (aSuccess)
            {
                m_textLoginStatus.text = "Login complete";
                Debug.Log("[ChatSystem] Vivox login success");
                m_rectPlayFabLogin.gameObject.SetActive(false);
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
                m_inputFieldNetworkId.text = m_chatServiceLogin.GetTokenId(m_inputFieldChannelName.text);
                
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
            if (m_chatServiceEvents != null)
            {
                m_chatServiceEvents.DeregisterOnUserConnectionChange(OnUserConnectStateChange);
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

            DependencyContainer.instance.RegisterToContainer<IChatSystem>(this);
            m_textLoginStatus.text = "Initializing Vivox";
            m_chatServiceLogin = new VivoxLogin((b =>
            {
                if (b)
                {
                    m_textLoginStatus.text = "Vivox initialized";
                    Debug.Log("[ChatSystem] Vivox initialization success");
                    m_textLoginStatus.text = "Logging into Vivox as "+m_strUserName;
                    m_chatServiceLogin.Login(m_strUserName);
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

            m_bCreatedChannel = true;
            m_chatServiceLogin.CreateAndJoinChannel(aChannelName, ChannelType.NonPositional, true, true, true, null);
        }

        public void JoinChannel(string aChannelToken, string aChannelName)
        {
            m_chatServiceLogin.JoinChannel(aChannelToken ,aChannelName, ChannelType.NonPositional, true, true, true, null);
        }

        public void SendChatMessageToAll(string aMessage)
        {
            m_chatServiceMessages.SendChatMessageToAll(aMessage,m_chatServiceLogin.AccountId);
        }

        private void OnUserConnectStateChange(IChannelUserData aChannelUserData)
        {
            Debug.Log("[ChatSystem] Vivox new user entered channel");
        }
        
       
    }
}
