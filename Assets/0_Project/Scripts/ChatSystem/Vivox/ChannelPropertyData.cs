using Chat;
using VivoxUnity;

public class ChannelPropertyData : IChannelPropertyData
{
    public string Username { get; }
    public ChannelId ChannelId { get; }
    public string Property { get; }

    public ChannelPropertyData(string aUsername, ChannelId aChannelId, string aProperty)
    {
        Username = aUsername;
        ChannelId = aChannelId;
        Property = aProperty;
    }
}
