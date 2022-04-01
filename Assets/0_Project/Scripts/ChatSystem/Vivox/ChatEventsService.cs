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
        private IChatLoginService _mChatLoginService;
        private IChannelSession m_channelSession;

        private System.Action<IChannelPropertyData> m_channelDataAction;
        private System.Action<IChannelUserData> m_channelUserAction;

        public ChatEventsService(IChatLoginService aChatLoginService, IChannelSession aChannelSession)
        {
            _mChatLoginService = aChatLoginService;
            SetChannel(aChannelSession);
        }

        public void SetChannel(IChannelSession aChannelSession)
        {
            m_channelSession = aChannelSession;
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

            if (participant.IsSelf)
            {
                BindChannelSessionHandlers(false, channelSession); //Unsubscribe from events here
                channelId = null;
                ILoginSession loginSession = _mChatLoginService.VivoxClient.GetLoginSession(_mChatLoginService.AccountId);
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
                SetChannel(channelSession);
                channelSession.Participants.AfterKeyAdded += OnParticipantAdded;
                channelSession.Participants.BeforeKeyRemoved += OnParticipantRemoved;
                channelSession.Participants.AfterValueUpdated += OnParticipantValueUpdated;
            }

            //Unsubscribing to the events
            else
            {
                // Participants
                SetChannel(channelSession);
                channelSession.Participants.AfterKeyAdded += OnParticipantAdded;
                channelSession.Participants.BeforeKeyRemoved += OnParticipantRemoved;
                channelSession.Participants.AfterValueUpdated += OnParticipantValueUpdated;
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
