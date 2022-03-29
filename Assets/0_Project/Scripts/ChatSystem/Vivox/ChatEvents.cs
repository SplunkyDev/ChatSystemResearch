using System;
using System.Collections;
using System.Collections.Generic;
using BSS.Octane.Chat.Vivox;
using UnityEngine;
using VivoxUnity;

public class ChatEvents : IChatServiceEvents, IDisposable
{
   private IChannelSession m_channelSession;
   
   public ChatEvents(IChannelSession aChannelSession)
   {
      SetChannel(aChannelSession);
   }
   
   public void SetChannel(IChannelSession aChannelSession)
   {
      m_channelSession = aChannelSession;
   }

   public void RegisterOnUserJoinedChannel(EventHandler<KeyEventArg<string>> aEvent)
   {
      m_channelSession.Participants.AfterKeyAdded += aEvent;
   }

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
