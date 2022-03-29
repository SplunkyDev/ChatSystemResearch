using VivoxUnity;

namespace BSS.Octane.Chat.Vivox
{
   public interface IChatServiceLogin
   {
      void Login(string aDisplayName);
      void Logout();
      void JoinChannel(string aChannelName, ChannelType aChannelType, bool aConnectAudio, bool aConnectText,
         bool aTransmissionSwitch = true, Channel3DProperties aProperties = null);
      void LeaveChannel(ChannelId aChannelIdToLeave);
   }
}
