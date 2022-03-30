using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BSS.Octane.Chat.Vivox
{
    public class ChatMessage : IMessage
    {
        public string Channel { get; }
        public string Sender { get; }
        public string Message { get; }

        public ChatMessage(string aChannel, string aSender, string aMessage)
        {
            Channel = aChannel;
            Sender = aSender;
            Message = aMessage;
        }
    }
}
