using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonLoginUser : UiButton
{
    #region Serialize fields
    [SerializeField] private InputField m_inputUserName;
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
            m_chatSystem.Login( m_inputUserName.text);
        }

        base.OnPointerUp(eventData);
        Debug.Log($"[UiButton] Button Clicked: {GetType()}");
    }
}
