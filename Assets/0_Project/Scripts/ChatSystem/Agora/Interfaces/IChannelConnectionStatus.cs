using agora_gaming_rtc;

public interface IChannelConnectionStatus 
{
    CONNECTION_STATE_TYPE eConnectionState { get; }
    CONNECTION_CHANGED_REASON_TYPE eConnectionChangeReason { get; }
}
