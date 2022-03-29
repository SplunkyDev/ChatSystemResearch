using BSS.Octane.Chat;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BSS.Octane
{
    public class ButtonSetLanguage : UiButton
    {
        #region Serialized fields

        [SerializeField] private string m_strLanguage;
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
                m_chatSystem.SetLanguage(m_strLanguage);
            }
            base.OnPointerUp(eventData);
            Debug.Log($"[UiButton] Button Clicked: {GetType()} and Language: {m_strLanguage}");
        }
    }
}
