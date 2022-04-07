using agora_gaming_rtc;

namespace Chat.Agora
{
    public class ChannelConnectionStatus : IChannelConnectionStatus
    {
        public CONNECTION_STATE_TYPE eConnectionState { get; }
        public CONNECTION_CHANGED_REASON_TYPE eConnectionChangeReason { get; }

        public ChannelConnectionStatus(CONNECTION_STATE_TYPE aEConnectionState, CONNECTION_CHANGED_REASON_TYPE aEConnectionChangeReason)
        {
            eConnectionState = aEConnectionState;
            eConnectionChangeReason = aEConnectionChangeReason;
        }
    }
}

