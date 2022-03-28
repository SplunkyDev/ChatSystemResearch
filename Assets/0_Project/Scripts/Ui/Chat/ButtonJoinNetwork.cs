using System;
using System.Collections;
using System.Collections.Generic;
using BSS.Octane.Chat;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BSS.Octane
{
    public class ButtonJoinNetwork : UiButton
    {
        #region Serialize fields
        [SerializeField] private InputField m_inputFieldNetworkId;
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
                m_chatSystem.ConnectToChatNetwork(m_inputFieldNetworkId.text);
            }
            base.OnPointerUp(eventData);
            Debug.Log($"[UiButton] Button Clicked: {GetType()}");
        }
    }
}
