using VivoxUnity;

namespace BSS.Octane.Chat.Vivox
{
   public interface IChatLoginService
   {
      public Client VivoxClient { get; }
      public AccountId AccountId { get; }
      
      void Login(string aDisplayName);
      void Logout();
      void CreateAndJoinChannel(string aChannelName, ChannelType aChannelType, bool aConnectAudio, bool aConnectText,
         bool aTransmissionSwitch = true, Channel3DProperties aProperties = null);
      void JoinChannel(string aTokenKey, string aChannelName, ChannelType aChannelType, bool aConnectAudio,
         bool aConnectText,
         bool aTransmissionSwitch = true, Channel3DProperties aProperties = null);
      void LeaveChannel(ChannelId aChannelIdToLeave);
      string GetTokenId(string aChannelName);
   }
}
