using agora_gaming_rtc;

namespace Chat.Agora
{
    public interface IChatLoginServices
    {
        void Login(string aDisplayName);
        void Logout();
        void CreateAndJoinChannel(string token, string channelId, string info, uint uid, ChannelMediaOptions options);
        void LeaveChannel();
    }
}
