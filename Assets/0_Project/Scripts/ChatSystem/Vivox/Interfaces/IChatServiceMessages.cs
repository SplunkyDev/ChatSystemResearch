using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VivoxUnity;

namespace BSS.Octane.Chat.Vivox
{
   public interface IChatServiceMessages
   {
      void Initialize(IChannelSession aChannelSession);

      void SendChatMessageToAll(string aMessage);

      //TODO: Send message to specific user
      void SendChatMessageToUser(string aMessage);

      //Triggered when a message is received
      void RegisterOnChatMessageReceived(System.Action<object, QueueItemAddedEventArgs<IChannelTextMessage>> aAction);
      void DeregisterOnChatMessageReceived(System.Action<object, QueueItemAddedEventArgs<IChannelTextMessage>> aAction);

   }
}
