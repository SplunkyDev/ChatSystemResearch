using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonCreateJoinVivoxChannel : UiButton
{
    #region Serialize fields
    [SerializeField] private InputField m_inputChannelName;
    #endregion

    #region Private fields

    private Chat.IChatSystem m_chatSystem;

    #endregion

    private void Start()
    {
        m_chatSystem = DependencyContainer.instance.GetFromContainer<Chat.IChatSystem>();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (m_chatSystem != null)
        {
            m_chatSystem.CreateAndJoinChannel( m_inputChannelName.text);
        }

        base.OnPointerUp(eventData);
        Debug.Log($"[UiButton] Button Clicked: {GetType()}");
    }
}

