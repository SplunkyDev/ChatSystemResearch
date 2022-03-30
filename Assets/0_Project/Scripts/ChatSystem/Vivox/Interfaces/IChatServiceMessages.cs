using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VivoxUnity;

namespace BSS.Octane.Chat.Vivox
{
   public interface IChatServiceMessages
   {
      void SetChannel(IChannelSession aChannelSession);

      void SendChatMessageToAll(string aMessage, AccountId aAccountID);

      //TODO: Send message to specific user
      void SendChatMessageToUser(string aMessage, AccountId aAccountID);

      //Triggered when a message is received
      void RegisterOnChatMessageReceived(System.Action<IMessage> aAction);
      void DeregisterOnChatMessageReceived(System.Action<IMessage> aAction);
      
   }
}
