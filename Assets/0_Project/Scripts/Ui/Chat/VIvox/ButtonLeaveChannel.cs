using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonLeaveChannel : UiButton
{
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
            m_chatSystem.LeaveChannel();
        }

        base.OnPointerUp(eventData);
        Debug.Log($"[UiButton] Button Clicked: {GetType()}");
    }
}
