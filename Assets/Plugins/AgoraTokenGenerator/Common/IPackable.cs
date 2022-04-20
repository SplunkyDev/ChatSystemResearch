namespace AgoraIO.AccessToken
{
    public interface IPackable
    {
        ByteBuf marshal(ByteBuf outBuf);
    }
}
