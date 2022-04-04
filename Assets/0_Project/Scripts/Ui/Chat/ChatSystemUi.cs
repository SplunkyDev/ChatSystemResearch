using System.Collections;
using System.Collections.Generic;
using Chat;
using Chat.Vivox;
using UnityEngine;

public class ChatSystemUi : MonoBehaviour
{
    #region Serialized fields
    [SerializeField] private GameObject m_gTextMessage,m_gContent;
    #endregion
    
    
     #region Private fields
     private IChatMessageService m_ChatMessageService;
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
        m_ChatMessageService = DependencyContainer.instance.GetFromContainer<IChatMessageService>();
    
        m_ChatMessageService.RegisterOnChatMessageReceived(OnChatMessageReceived);
    }
    

    private void OnDestroy()
    {
        if(m_ChatMessageService != null)
        {
            m_ChatMessageService.DeregisterOnChatMessageReceived(OnChatMessageReceived);
        }
    }

    private void OnChatMessageReceived(IMessage aMessage)
    {
        IMessageFormatter messageFormatter =
            Instantiate(m_gTextMessage, m_gContent.transform).GetComponent<IMessageFormatter>();
        messageFormatter.TextMessage(aMessage.Sender,aMessage.Message);
        Debug.Log($"[ChatUiSystem][OnChatMessageReceived] Message from {aMessage.Sender} Message: {aMessage.Message}");
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
