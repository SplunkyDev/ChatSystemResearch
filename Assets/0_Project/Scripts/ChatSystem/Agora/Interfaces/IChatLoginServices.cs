using agora_gaming_rtc;

namespace Chat.Agora
{
    public interface IChatLoginServices
    {
        void Login(string aDisplayName);
        void Logout();
        void CreateAndJoinChannel(string aTokenKey, string aChannelId, string aUsername, ChannelMediaOptions options);
        void LeaveChannel();
    }
}
