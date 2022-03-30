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
            
            //Registering to the current channel, there could be multiple channels 
            m_chatServiceEvents.RegisterOnUserJoinedChannel(OnParticipantAdded);
            m_chatServiceEvents.RegisterOnUserLeaveChannel(OnParticipantRemoved);
            m_chatServiceEvents.RegisterOnUserStateChangel(OnParticipantValueUpdated);
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
        
        private void OnDestroy()
        {
             if(m_chatServiceEvents != null)
             {
                 m_chatServiceEvents.DeregisterOnUserJoinedChannel(OnParticipantAdded);
                 m_chatServiceEvents.DeregisterOnUserLeaveChannel(OnParticipantRemoved);
                 m_chatServiceEvents.DeregisterOnUserStateChangel(OnParticipantValueUpdated);
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

            DependencyContainer.instance.RegisterToContainer<ChatSystem>(this);
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
            m_chatServiceLogin.Login(aChannelName);
        }

        public void JoinChannel(string aChannelToken)
        {
            
        }

        public void SendChatMessageToAll(string aMessage)
        {
            m_chatServiceMessages.SendChatMessageToAll(aMessage,m_chatServiceLogin.AccountId);
        }
        
        #region Events
        private void OnParticipantAdded(object sender, KeyEventArg<string> keyEventArg)
        {
            ValidateArgs(new object[] { sender, keyEventArg });

            IReadOnlyDictionary<string, IParticipant> source = sender as IReadOnlyDictionary<string, IParticipant>;
            IParticipant participant = source?[keyEventArg.Key];
            string  strUsername = participant?.Account.Name;
            ChannelId channelId = participant?.ParentChannelSession.Key;
            IChannelSession channelSession = participant?.ParentChannelSession;
            //Do what you want with the information
            Debug.Log($"[ChatSystem][OnParticipantAdded] Account name: {strUsername} Channel Id: {channelId} Channel Session: {channelSession} ");
        }
        
        private void OnParticipantRemoved(object sender, KeyEventArg<string> keyEventArg)
        {
            ValidateArgs(new object[] { sender, keyEventArg });

            IReadOnlyDictionary<string, IParticipant> source = sender as IReadOnlyDictionary<string, IParticipant>;
            IParticipant participant = source?[keyEventArg.Key];
            string  strUsername = participant?.Account.Name;
            ChannelId channelId = participant?.ParentChannelSession.Key;
            IChannelSession channelSession = participant?.ParentChannelSession;
            //Do what you want with the information
            
            if (participant.IsSelf)
            {
                BindChannelSessionHandlers(false, channelSession); //Unsubscribe from events here
                channelId = null;
                ILoginSession loginSession = m_chatServiceLogin.Client.GetLoginSession(m_chatServiceLogin.AccountId);
                loginSession.DeleteChannelSession(channelSession.Channel);
            }
        }
        
        private void OnParticipantValueUpdated(object sender, ValueEventArg<string, IParticipant> valueEventArg)
        {
            ValidateArgs(new object[] { sender, valueEventArg }); 

            IReadOnlyDictionary<string, IParticipant> source = sender as IReadOnlyDictionary<string, IParticipant>;
            IParticipant participant = source[valueEventArg.Key];

            string username = valueEventArg.Value.Account.Name;
            ChannelId channel = valueEventArg.Value.ParentChannelSession.Key;
            string property = valueEventArg.PropertyName;
            
            //TODO: Check this implementation 
            switch (property)
            {
                case "LocalMute":
                    break;
                default:
                    break;
            }
        }
        
        private void ValidateArgs(object[] objs)
        {
            foreach (var obj in objs)
            {
                if (obj == null)
                    throw new ArgumentNullException(objs.GetType().ToString(), "The callback argument contains null data");
            }
        }
        
        
        private void BindChannelSessionHandlers(bool doBind, IChannelSession channelSession)
        {
            //Subscribing to the events
            if (doBind)
            {
                // Participants
                channelSession.Participants.AfterKeyAdded += OnParticipantAdded;
                channelSession.Participants.BeforeKeyRemoved += OnParticipantRemoved;
                channelSession.Participants.AfterValueUpdated += OnParticipantValueUpdated;
            }

            //Unsubscribing to the events
            else
            {
                // Participants
                if(m_chatServiceEvents != null)
                {
                    m_chatServiceEvents.SetChannel(channelSession);
                    m_chatServiceEvents.DeregisterOnUserJoinedChannel(OnParticipantAdded);
                    m_chatServiceEvents.DeregisterOnUserLeaveChannel(OnParticipantRemoved);
                    m_chatServiceEvents.DeregisterOnUserStateChangel(OnParticipantValueUpdated);
                }
                
            }
        }
        #endregion
       
    }
}
