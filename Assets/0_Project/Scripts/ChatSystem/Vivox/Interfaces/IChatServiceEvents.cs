using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VivoxUnity;

namespace BSS.Octane.Chat.Vivox
{
    public interface IChatServiceEvents
    {
        void SetChannel(IChannelSession aChannelSession);

        //Triggered when a user joins the channel
        void RegisterOnUserJoinedChannel(System.EventHandler<KeyEventArg<string>> aEvent);
        void DeregisterOnUserJoinedChannel(System.EventHandler<KeyEventArg<string>> aEvent);

        //Triggered when user leaves the channel
        void RegisterOnUserLeaveChannel(System.EventHandler<KeyEventArg<string>> aEvent);
        void DeregisterOnUserLeaveChannel(System.EventHandler<KeyEventArg<string>> aEvent);

        //Trigger when a user's state changes 
        void RegisterOnUserStateChangel(System.EventHandler<ValueEventArg<string, IParticipant>> aEvent);
        void DeregisterOnUserStateChangel(System.EventHandler<ValueEventArg<string, IParticipant>> aEvent);

    }
}
