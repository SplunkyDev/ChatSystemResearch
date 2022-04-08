using System;
using System.Text;
using agora_gaming_rtc;
using Chat;
using Chat.Agora;
using UnityEngine;

/// <summary>
/// This uses Voice channel to send messages 
/// </summary>
public class AgoraChatMessageService : IChatMessageService, IDisposable
{
    #region Private fields
    private System.Action<IMessage> OnMessageReceived;
    private IRtcEngine m_rtcEngine;
    #endregion
    
    #region Public fields
    #endregion
    
    public AgoraChatMessageService(IRtcEngine aRtcEngine)
    {
        m_rtcEngine = aRtcEngine;

        m_rtcEngine.OnStreamMessage += OnStreamMessageReceived;
        m_rtcEngine.OnStreamMessageError += OnStreamMessageError;
    }

    private void OnStreamMessageReceived(uint userId, int streamId, byte[] data, int length)
    {
        string strMessage = System.Text.Encoding.UTF8.GetString (data);
        var userInfo = m_rtcEngine.GetUserInfoByUid(userId);
        //TODO:Look into getting which channel is the message being streamed
        ChatMessage chatMessage = new ChatMessage("", userInfo.userAccount, strMessage);
        OnMessageReceived?.Invoke(chatMessage);
    }
    
    private void OnStreamMessageError(uint userId, int streamId, int code, int missed, int cached)
    {
        
    }
    
    public void SendMessageToAll(string aMessage)
    {
        int streamId = 0;
        DataStreamConfig config = new DataStreamConfig();
        config.syncWithAudio = false;
        config.ordered = true;
        streamId = m_rtcEngine.CreateDataStream(config);
        if (streamId < 0)
        {
            Debug.Log("CreateDataStream failed!");
            return;
        }
        
        byte[] byteMessage = System.Text.Encoding.UTF8.GetBytes(aMessage);
        m_rtcEngine.SendStreamMessage(streamId,byteMessage);
    }

    public void SendMessageToSpecifUser(string aUsername, string aMessage)
    {
        throw new NotImplementedException();
    }

    public void RegisterOnChatMessageReceived(Action<IMessage> aEvent)
    {
        OnMessageReceived += aEvent;
    }

    public void DeregisterOnChatMessageReceived(Action<IMessage> aEvent)
    {
        OnMessageReceived -= aEvent;
    }

    public void Dispose()
    {
        m_rtcEngine.OnStreamMessage -= OnStreamMessageReceived;
    }
}
