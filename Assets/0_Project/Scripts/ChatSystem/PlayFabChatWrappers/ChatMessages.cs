using System;
using PlayFab.Party;
using UnityEngine;

namespace Chat.PlayFab
{
    public class ChatMessages : IMultiplayerChatEvents , IDisposable
    {
        private PlayFabMultiplayerManager m_playFabMultiplayerManager;

        public ChatMessages(PlayFabMultiplayerManager aPlayFabMultiplayerManager)
        {
            if (aPlayFabMultiplayerManager == null)
            {
                Debug.Log("[ChatNetwork] Instance of PlayFabMultiplayerManager NOT Found");
                return;
            }
            m_playFabMultiplayerManager = aPlayFabMultiplayerManager;
        }

        public void RegisterOnChatMesseageReceived(PlayFabMultiplayerManager.OnChatMessageReceivedHandler aEvent)
        {
            if(m_playFabMultiplayerManager == null)
                return;
            m_playFabMultiplayerManager.OnChatMessageReceived += aEvent;
        }

        public void DeregisterOnChatMessageReceived(PlayFabMultiplayerManager.OnChatMessageReceivedHandler aEvent)
        {
            if(m_playFabMultiplayerManager == null)
                return;
            m_playFabMultiplayerManager.OnChatMessageReceived -= aEvent;
        }

        public void RegisterOnMesseageReceived(PlayFabMultiplayerManager.OnDataMessageReceivedHandler aEvent)
        {
            if(m_playFabMultiplayerManager == null)
                return;
            m_playFabMultiplayerManager.OnDataMessageReceived += aEvent;
        }

        public void DeregisterOnMessageReceived(PlayFabMultiplayerManager.OnDataMessageReceivedHandler aEvent)
        {
            if(m_playFabMultiplayerManager == null)
                return;
            
            m_playFabMultiplayerManager.OnDataMessageReceived -= aEvent;
        }

        public void RegisterOnNoCopyMesseageReceived(PlayFabMultiplayerManager.OnDataMessageReceivedNoCopyHandler aEvent)
        {
            if(m_playFabMultiplayerManager == null)
                return;
            
            m_playFabMultiplayerManager.OnDataMessageNoCopyReceived += aEvent;
        }

        public void DeregisterOnNoCopyMessageReceived(PlayFabMultiplayerManager.OnDataMessageReceivedNoCopyHandler aEvent)
        {
            if(m_playFabMultiplayerManager == null)
                return;
            
            m_playFabMultiplayerManager.OnDataMessageNoCopyReceived -= aEvent;
        }

        public void Dispose()
        {
            m_playFabMultiplayerManager = null;
        }
    }
}
