using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BSS.Octane.Chat
{
    public interface IMessageFormatter
    {
        void TextMessage(string aStrSender, string aStrMessage);
    }
}
