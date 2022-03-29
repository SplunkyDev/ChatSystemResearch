using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VivoxUnity;

namespace BSS.Octane.Chat.Vivox
{
    public interface IChatServiceEvents
    {
        void Initialize(IChannelSession aChannelSession);

        //Triggered when a user joins the channel
        void RegisterOnUserJoinedChannel(System.Action<object, KeyEventArg<string>> aAction);
        void DeregisterOnUserJoinedChannel(System.Action<object, KeyEventArg<string>> aAction);

        //Triggered when user leaves the channel
        void RegisterOnUserLeaveChannel(System.Action<object, KeyEventArg<string>> aAction);
        void DeregisterOnUserLeaveChannel(System.Action<object, KeyEventArg<string>> aAction);

        //Trigger when a user's state changes 
        void RegisterOnUserStateChangel(System.Action<object, ValueEventArg<string, IParticipant>> aAction);
        void DeregisterOnUserStateChangel(System.Action<object, ValueEventArg<string, IParticipant>> aAction);

    }
}
