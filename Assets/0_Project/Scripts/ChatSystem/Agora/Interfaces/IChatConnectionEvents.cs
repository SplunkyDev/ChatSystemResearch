using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chat.Agora
{
    public interface IChatConnectionEvents
    {
        //Registering to join/Left channel callback
        void RegisterOnChannelJoinOrLeft(System.Action<IChannelUserStatus> aEvent); //Initial Join and Rejoin
        void DeregisterOnChannelJoinOrLeft(System.Action<IChannelUserStatus> aEvent);

        //Registering to connection lost callback
        void RegisterOnConnectionStatus(System.Action<IChannelConnectionStatus> aEvent); //ConnectionLost, Connection Interrupted, Connect banned, connection state change
        void DeregisterOnConnectionStatus(System.Action<IChannelConnectionStatus> aEvent);    
    }
}
    
