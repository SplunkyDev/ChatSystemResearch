using System.Collections;
using System.Collections.Generic;
using PlayFab.Party;

namespace Chat.PlayFab
{
    public interface IMultiplayerChat
    {
        /// <summary>
        /// Create a network and join the same network
        /// </summary>
        void CreateAndJoinNetwork();
        /// <summary>
        /// Join the network with a unique network id
        /// </summary>
        /// <param name="astrNetworkId"></param>
        void JoinNetwork(string astrNetworkId);

        /// <summary>
        /// Leave the current network
        /// </summary>
        void LeaveNetwork();
        
        //Reference code: https://docs.microsoft.com/en-us/azure/cognitive-services/speech-service/language-support
        /// <summary>
        /// Set the language for the local device
        /// </summary>
        /// <param name="aStrLanguage">Language code</param>
        void SetLanguage(string aStrLanguage);
        
        #region Messaging 
        /// <summary>
        /// Send message to all players in the network
        /// </summary>
        /// <param name="aStrMessage">String message</param>
        void SendDataMessageToAllPlayer(string aStrMessage);

        /// <summary>
        /// This uses the chat api to send messages to all players 
        /// </summary>
        /// /// <param name="aStrMessages">Message string</param>
        public void SendChatMessageToAllPlayer(string aStrMessage);

        /// <summary>
        /// This uses the chat api to send messages to players in the list
        /// </summary>
        /// <param name="aStrMessages">Message string</param>
        /// <param name="alistPlayFabPlayer">List of playfabplayer type</param>
        public void SendChatMessageToPlayer(string aStrMessages, IList<PlayFabPlayer> alistPlayFabPlayer);
        #endregion
        
        #region Player Info
        /// <summary>
        /// Returns a reference to the local player.
        /// </summary>
        public PlayFabPlayer GetLocalPlayerInfo();
        
        /// <summary>
        /// Contains a collection of the remote players currently joined to the network.
        /// </summary>
        public IEnumerable<PlayFabPlayer> GetRemotePlayerInfo();
        #endregion
        
        #region Event registry 
        
        #region Network Join/Left
        /// <summary>
        /// Register to an event that is raised when the local player has joined the network. 
        /// </summary>
        /// <param name="aEvent"> sender: object | networkId: string  </param>
        void RegisterOnNetworkJoined(PlayFabMultiplayerManager.OnNetworkJoinedHandler aEvent);
        /// <summary>
        /// Deregister from an event that is raised when the local player has joined the network.
        /// </summary>
        /// <param name="aEvent">sender: object | networkId: string </param>
        void DeregisterOnNetworkJoined(PlayFabMultiplayerManager.OnNetworkJoinedHandler aEvent);
        
        /// <summary>
        /// Register to an event that is raised when the local player has left the network  
        /// </summary>
        /// <param name="aEvent"> sender: object | networkId: string  </param>
        void RegisterOnNetworkLeft(PlayFabMultiplayerManager.OnNetworkLeftHandler aEvent);
        /// <summary>
        /// Deregister from an event that is raised when the local player has left the network 
        /// </summary>
        /// <param name="aEvent">sender: object | networkId: string </param>
        void DeregisterOnNetworkLeft(PlayFabMultiplayerManager.OnNetworkLeftHandler aEvent);
        
        /// <summary>
        /// Register to an event that is raised when the local player has joined the network. 
        /// </summary>
        /// <param name="aEvent"> sender: object | remote player info: PlayFabPlayer  </param>
        void RegisterOnRemotePlayerJoined(PlayFabMultiplayerManager.OnRemotePlayerJoinedHandler aEvent);
        /// <summary>
        /// Deregister from an event that is raised when the local player has joined the network.
        /// </summary>
        /// <param name="aEvent">sender: object |  remote player info: PlayFabPlayer </param>
        void DeregisterOnRemotePlayerJoined(PlayFabMultiplayerManager.OnRemotePlayerJoinedHandler aEvent);
        
        /// <summary>
        /// Register to an event that is raised when the local player has left the network  
        /// </summary>
        /// <param name="aEvent"> sender: object | remote player info: PlayFabPlayer  </param>
        void RegisterOnRemotePlayerLeft(PlayFabMultiplayerManager.OnRemotePlayerLeftHandler aEvent);
        /// <summary>
        /// Deregister from an event that is raised when the local player has left the network 
        /// </summary>
        /// <param name="aEvent">sender: object |  remote player info: PlayFabPlayer </param>
        void DeregisterOnRemotePlayerLeft(PlayFabMultiplayerManager.OnRemotePlayerLeftHandler aEvent);
        #endregion
        
        #endregion

        void CleanUp();
    }
}
