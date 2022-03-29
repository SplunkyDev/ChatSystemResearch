using System;
using PlayFab.Party;

namespace BSS.Octane.Chat.PlayFab
{
    public interface IMultiplayerChatEvents
    {
        #region Message received 
        /// <summary>
        /// Register to an event that is raised when a chat message is received.
        /// </summary>
        /// <param name="aEvent"> sender: object |  remote player info: PlayFabPlayer | The contents of the message: message | parameter that specifies the type of message: ChatMessageType </param>
        void RegisterOnChatMesseageReceived(PlayFabMultiplayerManager.OnChatMessageReceivedHandler aEvent);
        /// <summary>
        /// Deregister to an event that is raised when a chat message is received.
        /// </summary>
        /// <param name="aEvent"> sender: object |  remote player info: PlayFabPlayer | The contents of the message: message | parameter that specifies the type of message: ChatMessageType </param>
        void DeregisterOnChatMessageReceived(PlayFabMultiplayerManager.OnChatMessageReceivedHandler aEvent);
        
        /// <summary>
        /// Register to an event that is raised when a data message is received.
        /// </summary>
        /// <param name="aEvent"> sender: object |  remote player info: PlayFabPlayer | The data contents of the message: buffer </param>
        void RegisterOnMesseageReceived(PlayFabMultiplayerManager.OnDataMessageReceivedHandler aEvent);
        /// <summary>
        /// Deregister to an event that is raised when a data message is received.
        /// </summary>
        /// <param name="aEvent"> sender: object |  remote player info: PlayFabPlayer | The data contents of the message: buffer </param>
        void DeregisterOnMessageReceived(PlayFabMultiplayerManager.OnDataMessageReceivedHandler aEvent);
        
        /// <summary>
        /// Register to an event that is raised, A more advanced version of the OnDataMessageReceived event that avoids copies.
        /// </summary>
        /// <param name="aEvent"> sender: object |  remote player info: PlayFabPlayer | pointer of the message buffer: Intptr | Length of the buffer </param>
        void RegisterOnNoCopyMesseageReceived(PlayFabMultiplayerManager.OnDataMessageReceivedNoCopyHandler aEvent);
        /// <summary>
        /// Deregister to an event that is raised, A more advanced version of the OnDataMessageReceived event that avoids copies.
        /// </summary>
        /// <param name="aEvent"> sender: object |  remote player info: PlayFabPlayer | pointer of the message buffer: Intptr | Length of the buffer </param>
        void DeregisterOnNoCopyMessageReceived(PlayFabMultiplayerManager.OnDataMessageReceivedNoCopyHandler aEvent);
        
        #endregion
    }
}
