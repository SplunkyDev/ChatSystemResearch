using VivoxUnity;

namespace Chat
{
    public interface IChannelPropertyData
    {
        public string Username { get; }
        public ChannelId ChannelId { get; }
        public string Property { get; }
        
    }
}
