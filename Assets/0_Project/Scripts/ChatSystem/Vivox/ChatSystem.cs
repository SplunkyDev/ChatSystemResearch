using System;
using UnityEngine;
using UnityEngine.UI;

namespace BSS.Octane.Chat.Vivox
{
    public class ChatSystem : MonoBehaviour
    {
        #region Serialize fields

        [SerializeField] private InputField m_inputFieldNetworkId;
        [SerializeField] private Text m_textLoginStatus;
        [SerializeField] private RectTransform m_rectJoinNetworkUi, m_rectChatUi, m_rectPlayFabLogin;

        #endregion

        #region Private fields

        private IChatServiceLogin m_chatServiceLogin;
        private bool m_bLoginSuccess = false;

        #endregion

        private void Start()
        {
            DependencyContainer.instance.RegisterToContainer<ChatSystem>(this);
            m_chatServiceLogin = new VivoxLogin((b =>
            {
                if (b)
                {
                    Debug.Log("[ChatSystem] Vivox initialization success");
                    CreateAndJoinChannel("Bastion");
                }
                else
                {
                    Debug.LogError("[ChatSystem] Vivox initialization failed");
                }

            }));


        }

        public void CreateAndJoinNetwork()
        {
            if (!m_bLoginSuccess)
            {
                Debug.LogWarning("[ChatSystem] Login isn't complete yet");
                return;
            }
        }

        public void CreateAndJoinChannel(string aChannelName)
        {
            m_chatServiceLogin.Login(aChannelName);
        }

        public void JoinChannel()
        {
            
        }
    
    }
}
