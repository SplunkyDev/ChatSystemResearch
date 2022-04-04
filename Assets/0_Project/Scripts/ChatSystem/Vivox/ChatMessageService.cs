using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VivoxUnity;

namespace Chat.Vivox
{
    public class ChatMessageService : IChatMessageService, IDisposable
    {
        private IChannelSession m_channelSession;
        private System.Action<IMessage> onMessageReceived;
        
        public ChatMessageService(IChannelSession aChannelSession)
        {
            SetChannel(aChannelSession);
        }
        
        public void SetChannel(IChannelSession aChannelSession)
        {
            m_channelSession = aChannelSession;
            m_channelSession.MessageLog.AfterItemAdded += OnChatMessageReceived;
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

        public void RegisterOnChatMessageReceived(Action<IMessage> aAction)
        {
            onMessageReceived += aAction;
        }

        public void DeregisterOnChatMessageReceived(Action<IMessage> aAction)
        {
            onMessageReceived -= aAction;
        }

        private void OnChatMessageReceived(object sender, QueueItemAddedEventArgs<IChannelTextMessage> queueItemAddedEventArgs)
        {
            string channelName = queueItemAddedEventArgs.Value.ChannelSession.Channel.Name;
            string senderName = queueItemAddedEventArgs.Value.Sender.DisplayName;
            string message = queueItemAddedEventArgs.Value.Message;
            ChatMessage chatMessage = new ChatMessage(channelName,senderName,message);

            Debug.Log(channelName + ": " + senderName + ": " + message);
            
            onMessageReceived?.Invoke(chatMessage);
        }

        public void Dispose()
        {
            m_channelSession.MessageLog.AfterItemAdded -= OnChatMessageReceived;
            m_channelSession = null;
        }
    }
}
