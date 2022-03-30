using BSS.Octane;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace  BSS.Octane
{
    public class ButtonJoinVivoxNetwork  : UiButton
    {
        #region Serialize fields
        [SerializeField] private InputField m_inputFieldNetworkId;
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
            if(m_chatSystem != null)
            {
                m_chatSystem.JoinChannel(m_inputFieldNetworkId.text);
            }
            base.OnPointerUp(eventData);
            Debug.Log($"[UiButton] Button Clicked: {GetType()}");
        }
    }
}
