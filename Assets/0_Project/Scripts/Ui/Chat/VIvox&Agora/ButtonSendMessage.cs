
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if AGORA_DEV
using Chat.Agora;
#elif VIVOX_DEV
using Chat.Vivox;
#endif

namespace Chat
{
    public class ButtonSendMessage : UiButton
    {
        #region Serialize fields

        [SerializeField] private InputField m_inputFieldMessage;
        [SerializeField] private InputField m_inputFieldPeer;

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
                string strPeerUSerName = m_inputFieldPeer.text;
                if (string.IsNullOrEmpty(strPeerUSerName))
                {
                    m_chatSystem.SenChatMessageToSpecificUser(strPeerUSerName,m_inputFieldMessage.text);
                }
                else
                {
                    m_chatSystem.SendChatMessageToAll(m_inputFieldMessage.text);
                }
            }

            base.OnPointerUp(eventData);

        }
    }
}