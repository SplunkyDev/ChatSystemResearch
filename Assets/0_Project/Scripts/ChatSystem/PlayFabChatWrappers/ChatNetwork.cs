using System;
using System.Collections.Generic;
using System.Text;
using BSS.Octane.Chat.PlayFab;
using BSS.Octane.Multiplayer.Chat.PlayFab;
using PlayFab.Party;
using UnityEngine;

namespace BSS.Octane.Chat
{
    public class ChatNetwork : IMultiplayerChat 
    {
        #region Private fields 
        private PlayFabMultiplayerManager m_playFabMultiplayerManager;
        private IMultiplayerChatEvents m_chatMessages;
        private IMultiplayerChatErrorEvents m_chatErrors;

        private bool m_bCleanUpDone = false;
        #endregion
        public ChatNetwork()
        {
            m_bCleanUpDone = false;
            m_playFabMultiplayerManager = PlayFabMultiplayerManager.Get();
            if (m_playFabMultiplayerManager == null)
            {
                Debug.Log("[ChatNetwork] Instance of PlayFabMultiplayerManager NOT Found");
                return;
            }
            
            m_chatMessages = new ChatMessages(m_playFabMultiplayerManager);
            m_chatErrors = new ChatErrors(m_playFabMultiplayerManager);
            
            RegisterOnNetworkJoined(OnNetworkJoined);
            //Register to container 
            DependencyContainer.instance.RegisterToContainer<IMultiplayerChat>(this);
            DependencyContainer.instance.RegisterToContainer<IMultiplayerChatEvents>(m_chatMessages);
            DependencyContainer.instance.RegisterToContainer<IMultiplayerChatErrorEvents>(m_chatErrors);

        }
        
        private void OnNetworkJoined(object aSender, string aStrNetworkId)
        {
            // m_playFabMultiplayerManager.TranslateChat = true;
            // m_playFabMultiplayerManager.ModerateChat = true;
            // m_playFabMultiplayerManager.ModerateAndTranslateChat = true;
            m_playFabMultiplayerManager.LocalPlayer.VoiceLevel = 1;
            Debug.Log($"[ChatNetwork] Chat control available: {m_playFabMultiplayerManager.LocalPlayer.IsChatControlAvailable}" );
            Debug.Log($"[ChatNetwork] Chat Language code: {m_playFabMultiplayerManager.LocalPlayer.LanguageCode}" );
            // Debug.Log($"[ChatNetwork] translate chat: {m_playFabMultiplayerManager.TranslateChat}");
            // Debug.Log($"[ChatNetwork] moderated chat: {m_playFabMultiplayerManager.ModerateChat}");
            // Debug.Log($"[ChatNetwork] translate and moderated chat: {m_playFabMultiplayerManager.ModerateAndTranslateChat}");
        }
        
        public void CleanUp()
        {
            m_bCleanUpDone = true;
            DependencyContainer.instance.DeregisterFromContainer<IMultiplayerChat>(this);
            DependencyContainer.instance.DeregisterFromContainer<IMultiplayerChatEvents>(m_chatMessages);
            DependencyContainer.instance.DeregisterFromContainer<IMultiplayerChatErrorEvents>(m_chatErrors);
            m_playFabMultiplayerManager = null; m_chatMessages = null;  m_chatErrors = null;
        }
        
        public void CreateAndJoinNetwork()
        {
            m_playFabMultiplayerManager.CreateAndJoinNetwork();
        }

        public void JoinNetwork(string astrNetworkId)
        {
            m_playFabMultiplayerManager.JoinNetwork(astrNetworkId);
        }

        public void SetLanguage(string aStrLanguage)
        {
            m_playFabMultiplayerManager.LocalPlayer.LanguageCode = aStrLanguage;
        }
        
        public void LeaveNetwork()
        {
            m_playFabMultiplayerManager.LeaveNetwork();
        }
        
        public void SendDataMessageToAllPlayer(string aStrMessage)
        {
            // Simple send message.
            byte[] buffer = Encoding.ASCII.GetBytes(aStrMessage);
            m_playFabMultiplayerManager.SendDataMessageToAllPlayers(buffer);
        }

        public void SendChatMessageToAllPlayer(string aStrMessage)
        {
            m_playFabMultiplayerManager.SendChatMessageToAllPlayers(aStrMessage);
        }
        
        public void SendChatMessageToPlayer(string aStrMessages, IList<PlayFabPlayer> alistPlayFabPlayer)
        {
            m_playFabMultiplayerManager.SendChatMessage(aStrMessages,alistPlayFabPlayer);
        }

        public PlayFabPlayer GetLocalPlayerInfo()
        {
            return m_playFabMultiplayerManager.LocalPlayer;
        }

        public IEnumerable<PlayFabPlayer> GetRemotePlayerInfo()
        {
            return m_playFabMultiplayerManager.RemotePlayers;
        }
        
        
#region Events 
        public void RegisterOnNetworkJoined(PlayFabMultiplayerManager.OnNetworkJoinedHandler aEvent)
        {
            if(m_playFabMultiplayerManager == null)
                return;
                
            m_playFabMultiplayerManager.OnNetworkJoined += aEvent ;
        }

        public void DeregisterOnNetworkJoined(PlayFabMultiplayerManager.OnNetworkJoinedHandler aEvent)
        {
            if(m_playFabMultiplayerManager == null)
                return;
            
            m_playFabMultiplayerManager.OnNetworkJoined -= aEvent ;
        }

        public void RegisterOnNetworkLeft(PlayFabMultiplayerManager.OnNetworkLeftHandler aEvent)
        {
            if(m_playFabMultiplayerManager == null)
                return;
            
            m_playFabMultiplayerManager.OnNetworkLeft += aEvent ;
        }

        public void DeregisterOnNetworkLeft(PlayFabMultiplayerManager.OnNetworkLeftHandler aEvent)
        {
            if(m_playFabMultiplayerManager == null)
                return;
            
            m_playFabMultiplayerManager.OnNetworkLeft -= aEvent ;
        }

        public void RegisterOnRemotePlayerJoined(PlayFabMultiplayerManager.OnRemotePlayerJoinedHandler aEvent)
        {
            m_playFabMultiplayerManager.OnRemotePlayerJoined += aEvent ;
        }

        public void DeregisterOnRemotePlayerJoined(PlayFabMultiplayerManager.OnRemotePlayerJoinedHandler aEvent)
        {
            if(m_playFabMultiplayerManager == null)
                return;
            
            m_playFabMultiplayerManager.OnRemotePlayerJoined -= aEvent ;
        }

        public void RegisterOnRemotePlayerLeft(PlayFabMultiplayerManager.OnRemotePlayerLeftHandler aEvent)
        {
            if(m_playFabMultiplayerManager == null)
                return;
                
            m_playFabMultiplayerManager.OnRemotePlayerLeft += aEvent ;
        }

        public void DeregisterOnRemotePlayerLeft(PlayFabMultiplayerManager.OnRemotePlayerLeftHandler aEvent)
        {
            if(m_playFabMultiplayerManager == null)
                return;
            
            m_playFabMultiplayerManager.OnRemotePlayerLeft -= aEvent ;
        }
#endregion
    }
}
