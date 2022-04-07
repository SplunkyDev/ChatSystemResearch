namespace Chat.Agora
{
    public interface IChannelUserStatus
    {
        public string Username { get; }
        public uint UserId { get; }
        public bool ParticipantJoined { get; }
    }
}
