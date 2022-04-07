using System.Collections;
using System.Collections.Generic;
using Chat.Vivox;
using UnityEngine;
using VivoxUnity;

public class ChannelUserData : IChannelUserData
{
    public string Username { get; }
    public ChannelId ChannelId { get; }
    public IChannelSession ChannelSession { get; }

    public bool ParticipantJoined { get; }

    public ChannelUserData(string aUsername, ChannelId aChannelId, IChannelSession aChannelSession, bool aParticipantJoined )
    {
        Username = aUsername;
        ChannelId = aChannelId;
        ChannelSession = aChannelSession;
        ParticipantJoined = aParticipantJoined;
    }
}
