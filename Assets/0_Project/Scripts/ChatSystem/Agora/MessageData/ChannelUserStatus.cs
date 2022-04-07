
using Chat.Agora;

namespace Chat.Agora
{
    public class ChannelUserStatus : IChannelUserStatus
    {
        public string Username { get; }
        public uint UserId { get; }
        public bool ParticipantJoined { get; }

        public ChannelUserStatus(string aUsername, uint aUserId, bool aParticipant)
        {
            Username = aUsername;
            UserId = aUserId;
            ParticipantJoined = aParticipant;
        }
    }
}
