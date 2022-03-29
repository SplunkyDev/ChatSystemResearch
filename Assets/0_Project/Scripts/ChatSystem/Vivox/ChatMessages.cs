using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VivoxUnity;

namespace BSS.Octane.Chat.Vivox
{
    public class ChatMessages : IChatServiceMessages
    {
        private IChannelSession m_channelSession;
        
        public ChatMessages(IChannelSession aChannelSession)
        {
            SetChannel(aChannelSession);
        }
        
        public void SetChannel(IChannelSession aChannelSession)
        {
            m_channelSession = aChannelSession;
        }

        public void SendChatMessageToAll(string aMessage, AccountId aAccountID)
        {
            var channelName = m_channelSession.Channel.Name;

            m_channelSession.BeginSendText(aMessage, ar =>
            {
                try
                {
                    m_channelSession.EndSendText(ar);
                }
                catch (Exception e)
                {
                    // Handle error
                    return;
                }
                Debug.Log(channelName + ": " + aAccountID.Name + ": " + aMessage);
            });
        }

        public void SendChatMessageToUser(string aMessage, AccountId aAccountID)
        {
            throw new NotImplementedException();
        }

        public void RegisterOnChatMessageReceived(EventHandler<QueueItemAddedEventArgs<IChannelTextMessage>> aEvent)
        {
            m_channelSession.MessageLog.AfterItemAdded += aEvent;
        }

        public void DeregisterOnChatMessageReceived(EventHandler<QueueItemAddedEventArgs<IChannelTextMessage>> aEvent)
        {
            m_channelSession.MessageLog.AfterItemAdded -= aEvent;
        }
    }
}
