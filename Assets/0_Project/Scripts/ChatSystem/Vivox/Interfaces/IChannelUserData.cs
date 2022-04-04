
using VivoxUnity;

namespace Chat
{
    public interface IChannelUserData
    {
        public string Username { get; }
        public ChannelId ChannelId { get; }
        public IChannelSession ChannelSession { get; }
        public bool ParticipantJoined { get; }

        
    }
}
