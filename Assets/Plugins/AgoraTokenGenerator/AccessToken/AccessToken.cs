using Crc32C;
using System;
using System.IO;
using UnityEngine;
namespace AgoraIO.AccessToken
{
    public class AccessToken
    {
        private string _appId;
        private string _appCertificate;
        private string _channelName;
        private string _uid;
        private uint _ts;
        private uint _salt;
        private byte[] _signature;
        private uint _crcChannelName;
        private uint _crcUid;
        private byte[] _messageRawContent;
        public PrivilegeMessage message;

        public AccessToken(string appId, string appCertificate, string channelName, string uid)
        {
            _appId = appId;
            _appCertificate = appCertificate;
            _channelName = channelName;
            _uid = uid;
            
            Debug.Log($"<color=green>[{GetType()}]App Id: {_appId} App Cert: {_appCertificate} Channel Id: {_channelName} Username: {_uid}</color>");
            message = new PrivilegeMessage();
        }

        public AccessToken(string appId, string appCertificate, string channelName, string uid, uint ts, uint salt)
        {
            this._appId = appId;
            this._appCertificate = appCertificate;
            this._channelName = channelName;
            this._uid = uid;
            this.message = new PrivilegeMessage();
            this._ts = ts;
            this._salt = salt;
        }

        public void addPrivilege(Privileges aPrivileges, uint expiredTs)
        {
            this.message.messages.Add((ushort)aPrivileges, expiredTs);
        }

        public string build()
        {
            this._messageRawContent = Utils.pack(this.message);
            this._signature = generateSignature(_appCertificate
                    , _appId
                    , _channelName
                    , _uid
                    , _messageRawContent);
            
            this._crcChannelName = Crc32CAlgorithm.Compute(this._channelName.GetByteArray());
            this._crcUid = Crc32CAlgorithm.Compute(this._uid.GetByteArray());

            PackContent packContent = new PackContent(_signature, _crcChannelName, _crcUid, this._messageRawContent);
            byte[] content = Utils.pack(packContent);
            return getVersion() + this._appId + Utils.base64Encode(content);
        }
        public static String getVersion()
        {
            return "006";
        }

        public static byte[] generateSignature(String appCertificate
                , String appID
                , String channelName
                , String uid
                , byte[] message)
        {
            Debug.Log($"<color=green>[AccessToken]App Id: {appID} App Cert: {appCertificate} Channel Id: {channelName} Username: {uid}</color>");
            using (var ms = new MemoryStream())
            using (BinaryWriter baos = new BinaryWriter(ms))
            {
                baos.Write(appID.GetByteArray());
                baos.Write(channelName.GetByteArray());
                baos.Write(uid.GetByteArray());
                baos.Write(message);
                baos.Flush();

                byte[] sign = DynamicKeyUtil.encodeHMAC(appCertificate, ms.ToArray(), "SHA256");
                return sign;
            }
        }
    }
}
