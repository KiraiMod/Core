using System.Text;

namespace KiraiMod.Core.MessageAPI.Headers
{
    public class TargetHeader : MessageHeader
    {
        public override byte HeaderID => 49;

        public string[] Targets;

        public TargetHeader() { }
        public TargetHeader(params string[] Targets) => this.Targets = Targets;

        // unfortunately this will break with euan since they have a null byte in their name
        // fortunately they're a vrc developer so this code won't need to work for them
        public override byte[] Serialize() => Encoding.UTF8.GetBytes(string.Join('\0', Targets));
        public override void Deserialize(byte[] data) => Targets = Encoding.UTF8.GetString(data).Split('\0');
    }
}
