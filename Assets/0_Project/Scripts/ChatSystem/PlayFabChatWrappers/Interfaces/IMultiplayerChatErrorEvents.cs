using System;
using PlayFab.Party;


namespace Chat.PlayFab
{
    public interface IMultiplayerChatErrorEvents 
    {
        #region Event Errors
        /// <summary>
        /// Register to an event that is raised when there is an error.
        /// </summary>
        /// <param name="aEvent"> sender: object |  An object containing the details of the error: PlayFabMultiplayerManagerErrorArgs </param>
        void RegisterOnError(PlayFabMultiplayerManager.OnErrorEventHandler aEvent);
        /// <summary>
        /// Deregister to an event that is raised when there is an error.
        /// </summary>
        /// <param name="aEvent"> sender: object |  An object containing the details of the error: PlayFabMultiplayerManagerErrorArgs </param>
        void DeregisterOnError(PlayFabMultiplayerManager.OnErrorEventHandler aEvent);
        #endregion
    }
}
