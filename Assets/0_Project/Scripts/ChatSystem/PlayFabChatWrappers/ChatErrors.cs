using PlayFab.Party;
using UnityEngine;

namespace Chat.PlayFab
{
    public class ChatErrors : IMultiplayerChatErrorEvents
    {
        private PlayFabMultiplayerManager m_playFabMultiplayerManager;

        public ChatErrors(PlayFabMultiplayerManager aPlayFabMultiplayerManager)
        {
            if (aPlayFabMultiplayerManager == null)
            {
                Debug.Log("[ChatNetwork] Instance of PlayFabMultiplayerManager NOT Found");
                return;
            }
            m_playFabMultiplayerManager = aPlayFabMultiplayerManager;
        }

        public void RegisterOnError(PlayFabMultiplayerManager.OnErrorEventHandler aEvent)
        {
            m_playFabMultiplayerManager.OnError += aEvent;
        }

        public void DeregisterOnError(PlayFabMultiplayerManager.OnErrorEventHandler aEvent)
        {
            m_playFabMultiplayerManager.OnError -= aEvent;
        }
    }
}
