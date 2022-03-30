
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BSS.Octane.Chat
{
    public class ButtonSendVivoxMessage : UiButton
    {
        #region Serialize fields

        [SerializeField] private InputField m_inputFieldMessage;

        #endregion

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
                m_chatSystem.SendChatMessageToAll(m_inputFieldMessage.text);
            }

            base.OnPointerUp(eventData);

        }
    }
}