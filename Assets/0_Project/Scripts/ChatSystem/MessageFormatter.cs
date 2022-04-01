using Chat;
using UnityEngine;
using UnityEngine.UI;


public class MessageFormatter : MonoBehaviour, IMessageFormatter
{
    #region Serialized fields
    [SerializeField] private Text m_textSender, m_textMessage;
    #endregion

    private void Start()
    {
        m_textSender.color = Color.red;
        m_textMessage.color = Color.black;
    }

    public void TextMessage(string aStrSender, string aStrMessage)
    {
        m_textSender.text = aStrSender;
        m_textMessage.text = aStrMessage;
    }
}

