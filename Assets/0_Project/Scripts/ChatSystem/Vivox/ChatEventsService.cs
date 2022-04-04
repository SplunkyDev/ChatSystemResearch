using System;
using System.Collections;
using System.Collections.Generic;
using Chat.Vivox;
using UnityEngine;
using VivoxUnity;

namespace Chat.Vivox
{
    public class ChatEventsService : IChatEventsService, IDisposable
    {
        private IChatLoginService m_ChatLoginService;
        private IChannelSession m_channelSession;

        private System.Action<IChannelPropertyData> m_channelDataAction;
        private System.Action<IChannelUserData> m_channelUserAction;

        private List<string> m_lstChannelId = new List<string>();
        public ChatEventsService(IChatLoginService aChatLoginService, IChannelSession aChannelSession)
        {
            m_ChatLoginService = aChatLoginService;
            SetChannel(aChannelSession);
        }

        public void SetChannel(IChannelSession aChannelSession)
        {
            if( m_channelSession != null && m_channelSession == aChannelSession)
                return;
            
            m_channelSession = aChannelSession;
           if(!m_lstChannelId.Contains(m_channelSession.Channel.Name))
           {
               BindChannelSessionHandlers(true, m_channelSession);
           }
            
        }

        public void RegisterOnUserConnectionChange(Action<IChannelUserData> action)
        {
            m_channelUserAction += action;
        }

        public void DeregisterOnUserConnectionChange(Action<IChannelUserData> action)
        {
            m_channelUserAction -= action;
        }

        public void RegisterOnUserStateChange(Action<IChannelPropertyData> action)
        {
            m_channelDataAction += action;
        }

        public void DeregisterOnUserStateChange(Action<IChannelPropertyData> action)
        {
            m_channelDataAction += action;
        }


        #region Events

        private void OnParticipantAdded(object sender, KeyEventArg<string> keyEventArg)
        {
            ValidateArgs(new object[] {sender, keyEventArg});

            VivoxUnity.IReadOnlyDictionary<string, IParticipant> source =
                sender as VivoxUnity.IReadOnlyDictionary<string, IParticipant>;
            IParticipant participant = source?[keyEventArg.Key];
            string strUsername = participant?.Account.Name;
            ChannelId channelId = participant?.ParentChannelSession.Key;
            IChannelSession channelSession = participant?.ParentChannelSession;
            
            // Triggering ChannelUser event
            IChannelUserData channelUserData = new ChannelUserData(strUsername,channelId,channelSession, true);
            m_channelUserAction?.Invoke(channelUserData);
            
            //Do what you want with the information
            Debug.Log(
                $"[ChatSystem][OnParticipantAdded] Account name: {strUsername} Channel Id: {channelId} Channel Session: {channelSession} ");
        }

        private void OnParticipantRemoved(object sender, KeyEventArg<string> keyEventArg)
        {
            ValidateArgs(new object[] {sender, keyEventArg});

            VivoxUnity.IReadOnlyDictionary<string, IParticipant> source =
                sender as VivoxUnity.IReadOnlyDictionary<string, IParticipant>;
            IParticipant participant = source?[keyEventArg.Key];
            string strUsername = participant?.Account.Name;
            ChannelId channelId = participant?.ParentChannelSession.Key;
            IChannelSession channelSession = participant?.ParentChannelSession;
            //Do what you want with the information
            
            // Triggering ChannelUser event
            IChannelUserData channelUserData = new ChannelUserData(strUsername,channelId,channelSession, false);
            m_channelUserAction?.Invoke(channelUserData);
            
            if (participant.IsSelf)
            {
                BindChannelSessionHandlers(false, channelSession); //Unsubscribe from events here
                channelId = null;
                ILoginSession loginSession = m_ChatLoginService.VivoxClient.GetLoginSession(m_ChatLoginService.AccountId);
                loginSession.DeleteChannelSession(channelSession.Channel);
            }
        }

        private void OnParticipantValueUpdated(object sender, ValueEventArg<string, IParticipant> valueEventArg)
        {
            ValidateArgs(new object[] {sender, valueEventArg});

            VivoxUnity.IReadOnlyDictionary<string, IParticipant> source =
                sender as VivoxUnity.IReadOnlyDictionary<string, IParticipant>;
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
            
            // Triggering ChannelUser event
            IChannelPropertyData channelPropertyData = new ChannelPropertyData(username,channel,property);
            m_channelDataAction?.Invoke(channelPropertyData);
        }

        private void ValidateArgs(object[] objs)
        {
            foreach (var obj in objs)
            {
                if (obj == null)
                    throw new ArgumentNullException(objs.GetType().ToString(),
                        "The callback argument contains null data");
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
                m_lstChannelId.Add(channelSession.Channel.Name);
            }

            //Unsubscribing to the events
            else
            {
                // Participants
                SetChannel(channelSession);
                channelSession.Participants.AfterKeyAdded -= OnParticipantAdded;
                channelSession.Participants.BeforeKeyRemoved -= OnParticipantRemoved;
                channelSession.Participants.AfterValueUpdated -= OnParticipantValueUpdated;
                m_lstChannelId.Remove(channelSession.Channel.Name);
            }
        }

        #endregion

        public void DeregisterOnUserJoinedChannel(EventHandler<KeyEventArg<string>> aEvent)
        {
            m_channelSession.Participants.AfterKeyAdded -= aEvent;
        }

        public void RegisterOnUserLeaveChannel(EventHandler<KeyEventArg<string>> aEvent)
        {
            m_channelSession.Participants.BeforeKeyRemoved += aEvent;
        }

        public void DeregisterOnUserLeaveChannel(EventHandler<KeyEventArg<string>> aEvent)
        {
            m_channelSession.Participants.BeforeKeyRemoved -= aEvent;
        }

        public void RegisterOnUserStateChangel(EventHandler<ValueEventArg<string, IParticipant>> aEvent)
        {
            m_channelSession.Participants.AfterValueUpdated += aEvent;
        }

        public void DeregisterOnUserStateChangel(EventHandler<ValueEventArg<string, IParticipant>> aEvent)
        {
            m_channelSession.Participants.AfterValueUpdated -= aEvent;
        }

        public void Dispose()
        {
            m_channelSession = null;
        }
    }
}
