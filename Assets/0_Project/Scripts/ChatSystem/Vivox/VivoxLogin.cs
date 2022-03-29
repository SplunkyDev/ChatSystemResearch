using System;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Vivox;
using VivoxUnity;
using System.ComponentModel;
using UnityEngine;


namespace BSS.Octane.Chat.Vivox
{
    public class VivoxLogin : IChatServiceLogin
    {
        private ILoginSession m_LoginSession;
        private Dictionary<string,string> m_dictChannel =  new Dictionary<string,string>();
        
        public VivoxLogin(Action<bool> OnVivoxInitialized)
        {
            InitializeVivox(OnVivoxInitialized);
        }
        
        private async void InitializeVivox(Action<bool> OnVivoxInitialized)
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            VivoxService.Instance.Initialize();

            OnVivoxInitialized(VivoxService.Instance.IsAuthenticated);
            
            //TODO: Initialize Messenger and Events 
        }
        
        public void Login(string aDisplayName)
        {
            var account = new Account(aDisplayName);
            bool connectAudio = true;
            bool connectText = true;

            m_LoginSession = VivoxService.Instance.Client.GetLoginSession(account);
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
            if (aEvent.PropertyName == "State")
            {
                if (loginSession.State == LoginState.LoggedIn)
                {
                    bool connectAudio = true;
                    bool connectText = true;

                    // This puts you into an echo channel where you can hear yourself speaking.
                    // If you can hear yourself, then everything is working and you are ready to integrate Vivox into your project.
                    JoinChannel("TestChannel", ChannelType.Echo, connectAudio, connectText);
                    // To test with multiple users, try joining a non-positional channel.
                    // JoinChannel("MultipleUserTestChannel", ChannelType.NonPositional, connectAudio, connectText);
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
                channelSession.BeginConnect(aConnectAudio, aConnectText, aTransmissionSwitch, channelSession.GetConnectToken(), ar => 
                {
                    try
                    {
                        channelSession.EndConnect(ar);
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
    }
}
