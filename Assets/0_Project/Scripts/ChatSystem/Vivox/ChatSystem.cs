using System;
using System.Collections;
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
        [SerializeField] private RectTransform m_rectJoinNetworkUi, m_rectChatUi;
        [SerializeField] private RectTransform m_rectVivoxLogin;
        [SerializeField] private string m_strUserName;
        #endregion

        #region Private fields

        private IChatLoginService _mChatLoginService;
        private IChatMessageService _mChatMessageService;
        private IChatEventsService _mChatEventsService;

        private bool m_bLoginSuccess = false, m_bCreatedChannel = false;

        #endregion
        
        #region Public fields

        public bool ConnectionComplete { get; private set; }

        #endregion
        
        public void Inject(IChatEventsService aChatEventsService, IChatMessageService aChatMessageService)
        {
            _mChatEventsService = aChatEventsService;
            _mChatEventsService.RegisterOnUserConnectionChange(OnUserConnectStateChange);
            
            _mChatMessageService = aChatMessageService;
            
            ConnectionComplete = true;
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
                m_inputFieldNetworkId.text = _mChatLoginService.GetTokenId(m_inputFieldChannelName.text);
                
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
            if (_mChatEventsService != null)
            {
                _mChatEventsService.DeregisterOnUserConnectionChange(OnUserConnectStateChange);
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
            _mChatLoginService = new VivoxLogin((b =>
            {
                if (b)
                {
                    m_textLoginStatus.text = "Vivox initialized";
                    Debug.Log("[ChatSystem] Vivox initialization success");
                    m_textLoginStatus.text = "Logging into Vivox as "+m_strUserName;
                    _mChatLoginService.Login(m_strUserName);
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
            _mChatLoginService.CreateAndJoinChannel(aChannelName, ChannelType.NonPositional, true, true, true, null);
        }
        
        public void SendChatMessageToAll(string aMessage)
        {
            _mChatMessageService.SendChatMessageToAll(aMessage,_mChatLoginService.AccountId);
        }

        private void OnUserConnectStateChange(IChannelUserData aChannelUserData)
        {
            Debug.Log("[ChatSystem] Vivox new user entered channel");
        }
        
       
    }
}
