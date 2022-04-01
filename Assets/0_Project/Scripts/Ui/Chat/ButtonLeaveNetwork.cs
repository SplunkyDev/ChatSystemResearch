using Chat.PlayFabParty;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonLeaveNetwork : UiButton
{
    #region Private fields
    private ChatSystem m_chatSystem;
    #endregion

    private void Start()
    {
        m_chatSystem = DependencyContainer.instance.GetFromContainer<ChatSystem>();
    }
    
    public override void OnPointerUp(PointerEventData eventData)
    {
        if(m_chatSystem != null)
        {
            m_chatSystem.LeaveNetwork();
        }
        base.OnPointerUp(eventData);
        Debug.Log($"[UiButton] Button Clicked: {GetType()}");
    }
}

