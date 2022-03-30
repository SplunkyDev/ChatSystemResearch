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

        [SerializeField] private InputField m_inputFieldNetworkId;
        [SerializeField] private Text m_textLoginStatus;
        [SerializeField] private RectTransform m_rectJoinNetworkUi, m_rectChatUi, m_rectPlayFabLogin;

        #endregion

        #region Private fields

        private IChatServiceLogin m_chatServiceLogin;
        private IChatServiceMessages m_chatServiceMessages;
        private IChatServiceEvents m_chatServiceEvents;
        
        private bool m_bLoginSuccess = false;

        #endregion
        
        #region Public fields

        public bool ConnectionComplete { get; private set; }

        #endregion
        
        public void Inject(IChatServiceEvents aChatServiceEvents, IChatServiceMessages aChatServiceMessages)
        {
            m_chatServiceEvents = aChatServiceEvents;
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
                m_rectJoinNetworkUi.gameObject.SetActive(false);
                m_rectChatUi.gameObject.SetActive(true);
            }
            else
            {
                Debug.Log("[ChatSystem] Vivox channel not joined");
            }
        }


        private void OnDestroy()
        {
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
                    m_textLoginStatus.text = "Logging into Vivox as Jeeth";
                    m_chatServiceLogin.Login("Jeeth");
                }
                else
                {
                    Debug.LogError("[ChatSystem] Vivox initialization failed");
                }

            }), this);


        }

        public void CreateAndJoinNetwork()
        {
           
        }

        public void CreateAndJoinChannel(string aChannelName)
        {
            if (!m_bLoginSuccess)
            {
                Debug.LogWarning("[ChatSystem] Login isn't complete yet");
                return;
            }
            
          
            m_chatServiceLogin.CreateAndJoinChannel(aChannelName, ChannelType.NonPositional, true, true, true, null);
        }

        public void JoinChannel(string aChannelToken)
        {
            
        }

        public void SendChatMessageToAll(string aMessage)
        {
            m_chatServiceMessages.SendChatMessageToAll(aMessage,m_chatServiceLogin.AccountId);
        }
        
       
    }
}
