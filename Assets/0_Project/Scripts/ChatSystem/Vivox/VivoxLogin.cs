using System;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Vivox;
using VivoxUnity;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UIElements;


namespace BSS.Octane.Chat.Vivox
{
    public class VivoxLogin : IChatServiceLogin , IDisposable
    {
        private ILoginSession m_LoginSession;
        private AccountId m_accountId;
        private Dictionary<string,string> m_dictChannel =  new Dictionary<string,string>();
        private IChatServiceEvents m_chatServiceEvents;
        private IChatServiceMessages m_chatServiceMessages;
        private IChatSystem m_chatSystem;
        private bool m_bChatEssentialsInitialized = false;

        public Client Client { get; private set;  }
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
            VivoxService.Instance.Initialize();
            Client = VivoxService.Instance.Client;
            
            DependencyContainer.instance.RegisterToContainer<IChatServiceLogin>(this);
            OnVivoxInitialized(VivoxService.Instance.IsAuthenticated);
        }
        
        public void Login(string aDisplayName)
        {
            m_accountId = new Account(aDisplayName);
            bool connectAudio = true;
            bool connectText = true;

            m_LoginSession = VivoxService.Instance.Client.GetLoginSession(m_accountId);
            m_LoginSession.PropertyChanged += LoginSessionPropertyChange;                    

            m_LoginSession.BeginLogin(m_LoginSession.GetLoginToken(), SubscriptionMode.Accept, null, null, null, ar =>
            {
                try
                {   
                    m_LoginSession.EndLogin(ar);
                }
                catch (Exception e)
                {
                    // Unbind any login session-related events you might be subscribed to.
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

                        // This puts you into an echo channel where you can hear yourself speaking.
                        // If you can hear yourself, then everything is working and you are ready to integrate Vivox into your project.
                        JoinChannel("TestChannel", ChannelType.Echo, connectAudio, connectText);
                        // To test with multiple users, try joining a non-positional channel.
                        // JoinChannel("MultipleUserTestChannel", ChannelType.NonPositional, connectAudio, connectText);
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

        public void JoinChannel(string aChannelName, ChannelType aChannelType, bool aConnectAudio, bool aConnectText,
            bool aTransmissionSwitch = true, Channel3DProperties aProperties = null)
        {
            if (m_LoginSession.State == LoginState.LoggedIn)
            {
                Channel channel = new Channel(aChannelName, aChannelType, aProperties);
                IChannelSession channelSession = m_LoginSession.GetChannelSession(channel);
                string tokenKey = channelSession.GetConnectToken();
                m_dictChannel.Add(aChannelName,tokenKey);
                channelSession.BeginConnect(aConnectAudio, aConnectText, aTransmissionSwitch, tokenKey, ar => 
                {
                    try
                    {
                        channelSession.GetConnectToken(tokenKey, new TimeSpan(300));
                        channelSession.EndConnect(ar);
                        
                        //Making sure if a channel is being created for the first time, instantiate ChatEvents and ChatMessages, otherwise use the pre-existing instance
                        if(!m_bChatEssentialsInitialized)
                        {
                            m_bChatEssentialsInitialized = true;
                            m_chatServiceEvents = new ChatEvents(channelSession);
                            m_chatServiceMessages = new ChatMessages(channelSession);
                            DependencyContainer.instance.RegisterToContainer<IChatServiceEvents>(m_chatServiceEvents);
                            DependencyContainer.instance.RegisterToContainer<IChatServiceMessages>(
                                m_chatServiceMessages);
                           
                        }
                        else
                        {
                            m_chatServiceEvents.SetChannel(channelSession);
                            m_chatServiceMessages.SetChannel(channelSession);
                            //New channel, IChatSystem can register to the new channel as well. 
                            m_chatSystem.Inject(m_chatServiceEvents, m_chatServiceMessages);
                        }
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
            m_chatServiceEvents = null;
            m_chatServiceMessages = null;
            m_dictChannel.Clear();
        }
    }
}
