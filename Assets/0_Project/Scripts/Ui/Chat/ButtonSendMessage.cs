using System.Collections;
using System.Collections.Generic;
using BSS.Octane.Chat;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BSS.Octane
{
    public class ButtonSendMessage : UiButton
    {
        #region Serialize fields
        [SerializeField] private InputField m_inputFieldMessage;
        #endregion
        
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
                m_chatSystem.SendChatMessageToAllPlayer(m_inputFieldMessage.text);
            }
            base.OnPointerUp(eventData);
            
        }
    }
}
