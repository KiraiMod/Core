namespace KiraiMod.Core.MessageAPI
{
    public abstract class MessageHeader 
    {
        public abstract byte HeaderID { get; }
        public abstract byte[] Serialize();
        public abstract void Deserialize(byte[] data);

        public MessageHeader() { }
    }
}
