using UnityEngine;
using UnityEngine.EventSystems;
#if AGORA_DEV
using Chat.Agora;
#elif VIVOX_DEV
using Chat.Vivox;
#endif

public class ButtonLeaveChannel : UiButton
{
    #region Private fields

    private IChatSystem m_chatSystem;

    #endregion

    private void Start()
    {
        m_chatSystem = DependencyContainer.instance.GetFromContainer<IChatSystem>();
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
