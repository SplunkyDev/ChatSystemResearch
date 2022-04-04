using System;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Vivox;
using VivoxUnity;
using System.ComponentModel;
using UnityEngine;
using VivoxAccessToken;

namespace Chat.Vivox
{
    public class VivoxLogin : IChatLoginService , IDisposable
    {
        private ILoginSession m_LoginSession;
        private AccountId m_accountId;
        private Account m_account;
        private Dictionary<string,string> m_dictChannel =  new Dictionary<string,string>();
        private IChatEventsService _mChatEventsService;
        private IChatMessageService _mChatMessageService;
        private IChatSystem m_chatSystem;
        private bool m_bChatEssentialsInitialized = false;
        
        // original code
        public Client VivoxClient { get; private set;  } 
        // fix
        // public Client VivoxClient { get; private set;  } = new Client();
        //

        public AccountId AccountId { get => m_accountId; }

        public VivoxLogin(Action<bool> OnVivoxInitialized, IChatSystem aChatSystem)
        {
            InitializeVivox(OnVivoxInitialized);
            m_chatSystem = aChatSystem;
        }
        
        private async void InitializeVivox(Action<bool> OnVivoxInitialized)
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            // orginal code
            VivoxService.Instance.Initialize();
            VivoxClient = VivoxService.Instance.Client;
            // fix
            // VivoxClient.Uninitialize();
            // VivoxClient.Initialize();
            //

            DependencyContainer.instance.RegisterToContainer<IChatLoginService>(this);

            // original code
            OnVivoxInitialized(VivoxService.Instance.IsAuthenticated);
            // fix
            // OnVivoxInitialized(VivoxClient.Initialized);
            //
        }
        
       
        
        public void Login(string aDisplayName)
        {
            // original code
            m_account = new Account(aDisplayName);
            //m_accountId = new AccountId("13469-chat_-95312-test", aDisplayName, "mtu1xp.vivox.com");
            m_LoginSession = VivoxService.Instance.Client.GetLoginSession(m_account);
            string tokenKey =
                m_LoginSession.GetLoginToken("RUiLtKTgzZw5WPOF2IVrn2Bg6gK6lMnz", TimeSpan.FromSeconds(90));
            // fix
            // AccountId("issuer", "username", "domain");
            // all values are from Unity Game Services or from Vivox Developer portal , I used your Credentials
            // m_accountId = new AccountId("13469-chat_-95312-test", aDisplayName, "mtu1xp.vivox.com");
            // m_LoginSession = VivoxClient.GetLoginSession(m_accountId);
            //

            m_LoginSession.PropertyChanged += LoginSessionPropertyChange;                    

            // original code 
            m_LoginSession.BeginLogin(tokenKey, SubscriptionMode.Accept, null, null, null, ar =>
            // fix
            // m_LoginSession.BeginLogin(new Uri("https://unity.vivox.com/appconfig/13469-chat_-95312-test"),m_LoginSession.GetLoginToken("RUiLtKTgzZw5WPOF2IVrn2Bg6gK6lMnz", TimeSpan.FromSeconds(90)), SubscriptionMode.Accept, null, null, null, ar =>
            //
            {
                try
                {   
                    m_LoginSession.EndLogin(ar);
                }
                catch (Exception e)
                {
                    // Unbind any login session-related events you might be subscribed to.
                    m_LoginSession.PropertyChanged -= LoginSessionPropertyChange;
                    Debug.LogError($"[VivoxLogin][Login] Login error: {e.Message} callstack: {e.StackTrace}");
                    // Handle error
                    return;
                }
                // At this point, we have successfully requested to login. 
                // When you are able to join channels, LoginSession.State will be set to LoginState.LoggedIn.
                // Reference LoginSession_PropertyChanged()
            });
        }

        private void LoginSessionPropertyChange(object aSender, PropertyChangedEventArgs aEvent)
        {
            ILoginSession loginSession = aSender as ILoginSession;
            if (loginSession == null)
            {
                Debug.LogError($"[VivoxLogin]LoginSession is null");
                return;
            }
            if (aEvent.PropertyName == "State")
            {
                switch (loginSession?.State)
                {
                    case LoginState.LoggingIn:
                        // Operations as needed
                        break;
                    case LoginState.LoggedIn:
                        bool connectAudio = true;
                        bool connectText = true;
                        Debug.Log($"[Vivox][LoginSessionPropertyChange] login state: {loginSession.State} ");
                        m_chatSystem.OnLoginComplete(true);
                        break;
                    case LoginState.LoggedOut:
                        // Operations as needed
                        break;
                    default:
                        // Operations as needed
                        break;
                }
                
            }
        }

        public void Logout()
        {
            
        }
        
        public void CreateAndJoinChannel(string aChannelName, ChannelType aChannelType, bool aConnectAudio, bool aConnectText,
            bool aTransmissionSwitch = true, Channel3DProperties aProperties = null)
        {
            if (m_LoginSession.State == LoginState.LoggedIn)
            {
                
                Debug.Log($"[Vivox][JoinChannel] Channel Count: {m_LoginSession.ChannelSessions.Count} ");
                foreach (var channels in m_LoginSession.ChannelSessions)
                {
                    Debug.Log($"Channel Session: {channels}");
                }
                
                Debug.Log($"[Vivox][JoinChannel] Channel Name: {aChannelName} Channel Type: {aChannelType} ");
                Channel channel = new Channel(aChannelName, aChannelType, aProperties);
                IChannelSession channelSession = m_LoginSession.GetChannelSession(channel);
                // Subscribe to property changes for all channels.
                channelSession.PropertyChanged += OnChannelPropertyChanged;
                string tokenKey = channelSession.GetConnectToken("RUiLtKTgzZw5WPOF2IVrn2Bg6gK6lMnz",TimeSpan.FromSeconds(90));
                
                //https://docs.vivox.com/v5/general/unity/15_1_160000/en-us/access-token-guide/generate-token-secure-server/example-c-sharp.htm?tocpath=Vivox%20Unity%20SDK%20documentation%7CAccess%20Token%20Developer%20Guide%7CGenerate%20a%20token%20on%20a%20secure%20server%7C_____2
                // tokenKey = AccessToken.Token_f("RUiLtKTgzZw5WPOF2IVrn2Bg6gK6lMnz",
                //     "13469-chat_-95312-test",
                //     AccessToken.SecondsSinceUnixEpochPlusDuration(TimeSpan.FromSeconds(90)),
                //     "join",
                //     UnityEngine.Random.Range(0, 999),
                //     "unmute",
                //     "sip:"+m_accountId.DisplayName+".@mtu1xp.vivox.com", "sip:confctl-g-"+aChannelName+".@mtu1xp.vivox.com"
                // );
                
                m_dictChannel.Add(aChannelName,tokenKey);
                Debug.Log($"[Vivox][JoinChannel]Begin connection: Token Key: {tokenKey} and Channel Name: {aChannelName} ");
                channelSession.BeginConnect(aConnectAudio, aConnectText, aTransmissionSwitch, tokenKey, ar => 
                {
                    try
                    {
                        channelSession.EndConnect(ar);
                        m_chatSystem.OnChannelJoined(ar.IsCompleted);
                        Debug.Log($"[Vivox][JoinChannel] Channel Count: {m_LoginSession.ChannelSessions.Count} ");
                        //Making sure if a channel is being created for the first time, instantiate ChatEvents and ChatMessages, otherwise use the pre-existing instance
                        if(!m_bChatEssentialsInitialized)
                        {
                            m_bChatEssentialsInitialized = true;
                            //Creates intance of IChatEventsService, this instance register to events of the CHannelSession
                            _mChatEventsService = new ChatEventsService(this, channelSession);
                            //Creates instance if ChatMessageService, this implements the messaging API
                            _mChatMessageService = new ChatMessageService(channelSession);
                            //Registering the instances to the dependency container so that other services gcan get their reference if required
                            DependencyContainer.instance.RegisterToContainer<IChatEventsService>(_mChatEventsService);
                            DependencyContainer.instance.RegisterToContainer<IChatMessageService>(
                                _mChatMessageService);
                           
                        }
                        else
                        {
                            _mChatEventsService.SetChannel(channelSession);
                            _mChatMessageService.SetChannel(channelSession);
                        }
                        
                        //New channel, IChatSystem can register to the new channel as well. 
                        m_chatSystem.Inject(_mChatEventsService, _mChatMessageService);
                    }
                    catch(Exception e)
                    {
                        Debug.LogError($"Could not connect to channel: {e.Message}");
                        return;
                    }
                });
            } 
            else 
            {
                Debug.LogError("Can't join a channel when not logged in.");
            }
        }

        private void OnChannelPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            var channelSession = (IChannelSession)sender;

            Debug.Log(
                $"[VivoxLogin][OnChannelPropertyChanged] Property change: {propertyChangedEventArgs.PropertyName}");
            // This example only checks for AudioState changes.
            if (propertyChangedEventArgs.PropertyName == "AudioState")
            {
                switch (channelSession.AudioState)
                {
                    case ConnectionState.Connecting:
                        Debug.Log("Audio connecting in " + channelSession.Key.Name);
                        break;

                    case ConnectionState.Connected:
                        Debug.Log("Audio connected in " + channelSession.Key.Name);
                        break;

                    case ConnectionState.Disconnecting:
                        Debug.Log("Audio disconnecting in " + channelSession.Key.Name);
                        break;

                    case ConnectionState.Disconnected:
                        Debug.Log("Audio disconnected in " + channelSession.Key.Name);
                        break;
                }
            }
            else  if (propertyChangedEventArgs.PropertyName == "TextState")
            {
                switch (channelSession.TextState)
                {
                    case ConnectionState.Connecting:
                        Debug.Log("Text connecting in " + channelSession.Key.Name);
                        break;

                    case ConnectionState.Connected:
                        Debug.Log("Text connected in " + channelSession.Key.Name);
                        break;

                    case ConnectionState.Disconnecting:
                        Debug.Log("Text disconnecting in " + channelSession.Key.Name);
                        break;

                    case ConnectionState.Disconnected:
                        Debug.Log("Text disconnected in " + channelSession.Key.Name);
                        break;
                }
            }
            else if (propertyChangedEventArgs.PropertyName == "ChannelState")
            {
                switch (channelSession.ChannelState)
                {
                    case ConnectionState.Connecting:
                        Debug.Log("Channel connecting in " + channelSession.Key.Name);
                        break;

                    case ConnectionState.Connected:
                        Debug.Log("Channel connected in " + channelSession.Key.Name);
                        break;

                    case ConnectionState.Disconnecting:
                        Debug.Log("Channel disconnecting in " + channelSession.Key.Name);
                        break;

                    case ConnectionState.Disconnected:
                        Debug.Log("Channel disconnected in " + channelSession.Key.Name);
                        break;
                }
            }
        }

        public void LeaveChannel(ChannelId aChannelIdToLeave)
        {
            
        }

        public string GetTokenId(string aChannelName)
        {
            if (!m_dictChannel.ContainsKey(aChannelName))
            {
                Debug.LogError($"[VivoxLogin] {aChannelName} Channel not present");
                return String.Empty;
            }
            return m_dictChannel[aChannelName];
        }
        
       
        
        public void Dispose()
        {
            m_LoginSession = null;
            _mChatEventsService = null;
            _mChatMessageService = null;
            m_dictChannel.Clear();
        }
    }
}
