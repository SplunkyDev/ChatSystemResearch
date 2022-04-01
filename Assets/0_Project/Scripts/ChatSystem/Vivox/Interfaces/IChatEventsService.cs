using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VivoxUnity;

namespace BSS.Octane.Chat
{
    public interface IChatEventsService
    {
        void SetChannel(IChannelSession aChannelSession);

        //Triggered when a user joins/leaves the channel
        void RegisterOnUserConnectionChange(System.Action<IChannelUserData> action);
        void DeregisterOnUserConnectionChange(System.Action<IChannelUserData> action);
        
        //Trigger when a user's state changes 
        void RegisterOnUserStateChange(System.Action<IChannelPropertyData> action);
        void DeregisterOnUserStateChange(System.Action<IChannelPropertyData> action);

    }
}
