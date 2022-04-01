using System;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using Chat.PlayFab;
using PlayFab.Party;
using UnityEngine;
using UnityEngine.UI;

namespace Chat.PlayFabParty
{
    public class ChatSystem : MonoBehaviour
    {
        #region Serialize fields
        [SerializeField] private InputField m_inputFieldNetworkId;
        [SerializeField] private Text m_textLoginStatus;
        [SerializeField] private RectTransform m_rectJoinNetworkUi, m_rectChatUi, m_rectPlayFabLogin;
        #endregion
        
        #region Private fields
        private IMultiplayerChat m_multiplayerChat;
        private bool m_bLoginSuccess = false, m_bNetworkCreater;
        #endregion
        
        private void Awake()
        {
        }

        private void OnDestroy()
        {
            DependencyContainer.instance.DeregisterFromContainer<ChatSystem>(this);
            
            #region Deegister to essential events
            m_multiplayerChat.DeregisterOnNetworkJoined(OnNetworkJoined);
            m_multiplayerChat.DeregisterOnNetworkLeft(OnNetworkLeft);
            m_multiplayerChat.DeregisterOnRemotePlayerJoined(OnRemoteJoined);
            m_multiplayerChat.DeregisterOnRemotePlayerLeft(OnRemoteLeft);
            #endregion
            
            m_multiplayerChat.CleanUp();
        }

        private void Start()
        {
            DependencyContainer.instance.RegisterToContainer<ChatSystem>(this);
            
            m_bNetworkCreater = m_bLoginSuccess = false;
            m_multiplayerChat = new ChatNetwork();
            
            #region Register to essential events
            m_multiplayerChat.RegisterOnNetworkJoined(OnNetworkJoined);
            m_multiplayerChat.RegisterOnNetworkLeft(OnNetworkLeft);
            m_multiplayerChat.RegisterOnRemotePlayerJoined(OnRemoteJoined);
            m_multiplayerChat.RegisterOnRemotePlayerLeft(OnRemoteLeft);
            #endregion
#if PLATFORM_ANDROID
            if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
            {
                Permission.RequestUserPermission(Permission.Microphone);
            }
#endif
            
            LoginToBackend();
        }

        private void OnNetworkJoined(object aSender, string aStrNetworkId)
        {
            Debug.Log($"[ChatSystem] Logged into PlayFab Party. Network ID: {aStrNetworkId}");
            m_inputFieldNetworkId.text = aStrNetworkId;
            if(!m_bNetworkCreater)
            {
                m_rectJoinNetworkUi.gameObject.SetActive(false);
                m_rectChatUi.gameObject.SetActive(true);
            }
        }
        
        private void OnNetworkLeft(object aSender, string aStrNetworkId)
        {
            Debug.Log($"[ChatSystem] Logged out of PlayFab Party. Network ID: {aStrNetworkId}");
            m_inputFieldNetworkId.text = String.Empty;
            
            m_rectJoinNetworkUi.gameObject.SetActive(true);
            m_rectChatUi.gameObject.SetActive(false);
        }

        private void OnRemoteJoined(object aSender, PlayFabPlayer aPlayFabPlayer)
        {
            Debug.Log($"[ChatSystem] Remote player Logged into PlayFab Party. Entity Key: {aPlayFabPlayer.EntityKey}");
            if(m_bNetworkCreater)
            {
                m_rectJoinNetworkUi.gameObject.SetActive(false);
                m_rectChatUi.gameObject.SetActive(true);
            }
        }
        
        private void OnRemoteLeft(object aSender, PlayFabPlayer aPlayFabPlayer)
        {
            Debug.Log($"[ChatSystem] Remote player Logged off PlayFab Party. Entity Key: {aPlayFabPlayer.EntityKey}");
        }
        
        public void LoginToBackend()
        {

            m_rectPlayFabLogin.gameObject.SetActive(true);
            
            //This will be handled by backend. JUST DEMO PURPOSE
            // Log into playfab. The SDK will use the logged in user when connecting to the network.
            var request = new LoginWithCustomIDRequest { CustomId = UnityEngine.Random.value.ToString(), CreateAccount = true };
            PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);

        }
        
        private void OnLoginSuccess(LoginResult result)
        {
            Debug.Log("[ChatSystem] Logged into PlayFab.");
            m_bLoginSuccess = true;
            m_rectPlayFabLogin.gameObject.SetActive(false);
            m_rectJoinNetworkUi.gameObject.SetActive(true);
        }
        private void OnLoginFailure(PlayFabError error)
        {
            Debug.Log("[ChatSystem] Error logging into PlayFab: " + error.ErrorMessage);
            m_bLoginSuccess = false;
            m_textLoginStatus.text = "Login to PlayFab FAILED, Restart App";
        }

        public void SetLanguage(string aStrLanguage)
        {
          m_multiplayerChat.SetLanguage(aStrLanguage);
        }
        
        public void CreateChatNetwork()
        {
            if(!m_bLoginSuccess)
            {
                Debug.LogError("[ChatSystem] Login error cannot initiate playfab party connection");
                return;
            }

            m_bNetworkCreater = true;
            m_multiplayerChat.CreateAndJoinNetwork();
        }
        
        public void ConnectToChatNetwork(string aStrNetworkId)
        {
            if(!m_bLoginSuccess)
            {
                Debug.LogError("[ChatSystem] Login error cannot initiate playfab party connection");
                return;
            }
            m_multiplayerChat.JoinNetwork(aStrNetworkId);
        }

        public void LeaveNetwork()
        {
            m_multiplayerChat.LeaveNetwork();
        }
        
        public void SendMessageToAllPlayers(string aStrMessage)
        {
          m_multiplayerChat.SendDataMessageToAllPlayer(aStrMessage);
        }

        /// <summary>
        /// This uses the chat api to send messages to all players 
        /// </summary>
        /// /// <param name="aStrMessages">Message string</param>
        public void SendChatMessageToAllPlayer(string aStrMessage)
        {
            IList<PlayFabPlayer> allPlayers = new List<PlayFabPlayer>();
            foreach (var item in m_multiplayerChat.GetRemotePlayerInfo())
            {
                allPlayers.Add(item);
            }
            
            SendMessageToPlayer(aStrMessage,allPlayers);
            
        }
        
        /// <summary>
        /// This uses the chat api to send messages to players in the list
        /// </summary>
        /// <param name="aStrMessages">Message string</param>
        /// <param name="listPlayFabPlayer">List of playfabplayer type</param>
        public void SendMessageToPlayer(string aStrMessages, IList<PlayFabPlayer> alistPlayFabPlayer)
        {
            m_multiplayerChat.SendChatMessageToPlayer(aStrMessages, alistPlayFabPlayer);
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                Application.Quit();
            }
        }
        
    }
}
