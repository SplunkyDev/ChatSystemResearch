﻿using System;
using PartyCSharpSDK.Interop;

namespace PartyCSharpSDK
{
    // typedef enum PARTY_DEVICE_CONNECTION_TYPE
    // {
        // PARTY_DEVICE_CONNECTION_TYPE_RELAY_SERVER = 0,
        // PARTY_DEVICE_CONNECTION_TYPE_DIRECT_PEER_CONNECTION = 1,
    // } PARTY_DEVICE_CONNECTION_TYPE;
    public enum PARTY_DEVICE_CONNECTION_TYPE : UInt32
    {
        PARTY_DEVICE_CONNECTION_TYPE_RELAY_SERVER = 0,
        PARTY_DEVICE_CONNECTION_TYPE_DIRECT_PEER_CONNECTION = 1,
    }
}