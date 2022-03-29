using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using BSS.Octane.Chat;
using BSS.Octane.Chat.PlayFab;
using PlayFab.Party;
using UnityEngine;

namespace BSS.Octane
{
    public class ChatUiSystem : MonoBehaviour
    {
        #region Serialized fields

        [SerializeField] private GameObject m_gTextMessage,m_gContent;
        #endregion
        
        #region Private fields
        private IMultiplayerChatEvents m_multiplayerChatEvents;
        private string m_strLogMessage, m_strLogCallStack;
        private List<string> m_lstLogMessages = new List<string>();
        #endregion
        
        void OnEnable()
        {
            Application.logMessageReceived += HandleLog;
        }

        void OnDisable()
        {
            Application.logMessageReceived -= HandleLog;
        }

        void HandleLog(string logString, string stackTrace, LogType type)
        {
            m_strLogMessage = logString;
            m_strLogCallStack = stackTrace;
            m_lstLogMessages.Add(logString);
        }
        
        private IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();
            m_multiplayerChatEvents = DependencyContainer.instance.GetFromContainer<IMultiplayerChatEvents>();
            
            m_multiplayerChatEvents.RegisterOnMesseageReceived(OnMessageReceived);
            m_multiplayerChatEvents.RegisterOnChatMesseageReceived(OnChatMessageReceived);
        }

        private void OnDestroy()
        {
            m_multiplayerChatEvents.DeregisterOnMessageReceived(OnMessageReceived);
            m_multiplayerChatEvents.DeregisterOnChatMessageReceived(OnChatMessageReceived);
        }

        private void OnMessageReceived(object aSender, PlayFabPlayer aPlayFabPlayer, byte[] aBuffer)
        {
            string strMessage = Encoding.ASCII.GetString(aBuffer);
            if (string.IsNullOrEmpty(strMessage))
            {
                Debug.LogError("[ChatUiSystem][OnMessageReceived] Message string is empty");
                return;
            }
            Debug.Log($"[ChatUiSystem][OnMessageReceived] Message from {aPlayFabPlayer.EntityKey} content: {strMessage}");
        }

        private void OnChatMessageReceived(object aSender, PlayFabPlayer aPlayFabPlayer, string aStrMessage, ChatMessageType aChatMessageType)
        {
            IMessageFormatter messageFormatter =
                Instantiate(m_gTextMessage, m_gContent.transform).GetComponent<IMessageFormatter>();
            messageFormatter.TextMessage(aPlayFabPlayer.EntityKey.Id,aStrMessage);
            Debug.Log($"[ChatUiSystem][OnChatMessageReceived] Message from {aPlayFabPlayer.EntityKey.Id} content: {aStrMessage}");
        }

        private void OnGUI()
        {
            if (m_lstLogMessages.Count <= 0)
                return;
            
            // only display max the 5 latest log messages
            int maxMessages = Mathf.Min(5, m_lstLogMessages.Count);
            GUILayout.BeginArea(new Rect(Screen.width / 2 + 100, Screen.height - 200, 400, 200), GUI.skin.box);
            for (int i = (m_lstLogMessages.Count - 1); i >= (m_lstLogMessages.Count - maxMessages); --i)
            {
                GUILayout.Label(m_lstLogMessages[i]);
            }

            GUILayout.EndArea();
        }
    }
}
