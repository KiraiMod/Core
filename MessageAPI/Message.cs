using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KiraiMod.Core.MessageAPI
{
    public struct Message
    {
        private static byte[] CreateHeader(uint id) => new byte[8] { 0x53, 0x06, (byte)id, (byte)(id >> 8), (byte)(id >> 16), (byte)(id >> 24), 0x00, 0x00 };
        public static void Send(uint id) => Send(CreateHeader(id));
        public static void Send(uint id, string data) => Send(id, Encoding.UTF8.GetBytes(data));
        public static void Send(uint id, byte[] data) => Send(CreateHeader(id).Concat(data ?? Array.Empty<byte>()).ToArray());
        public static void Send(byte[] data)
        {
            if (Managers.RPCManager.Active)
                VRC.SDKBase.Networking.SceneEventHandler.TriggerEvent(new VRC.SDKBase.VRC_EventHandler.VrcEvent
                {
                    EventType = VRC.SDKBase.VRC_EventHandler.VrcEventType.SendRPC,
                    Name = "SendRPC",
                    ParameterObject = VRC.SDKBase.Networking.SceneEventHandler.gameObject,
                    ParameterString = "UdonSyncRunProgramAsRPC", // this should be changed since worlds probably won't be able to send the raw bytes anymore
                    ParameterBytes = new(data),
                }, VRC.SDKBase.VRC_EventHandler.VrcBroadcastType.AlwaysUnbuffered, VRC.SDKBase.Networking.LocalPlayer.gameObject);
        }

        public static void On(uint id, Action<Types.Player, Message> listener)
        {
            if (Managers.RPCManager.listeners.TryGetValue(id, out Action<Types.Player, Message> existing))
                Managers.RPCManager.listeners[id] = existing + listener;
            else Managers.RPCManager.listeners[id] = listener;
        }

        public string BodyString
        {
            get => Encoding.UTF8.GetString(Body);
        }

        public uint ID;
        public Dictionary<byte, byte[]> Headers = new();
        public byte[] Body = Array.Empty<byte>();

        public Message(uint ID) => this.ID = ID;
        public Message(uint ID, MessageHeader[] Headers) : this(ID, Headers, Array.Empty<byte>()) { }
        public Message(uint ID, MessageHeader[] Headers, byte[] Body) : this(ID, Headers.Select(x => (x.HeaderID, x.Serialize())).ToArray(), Body) { }
        public Message(uint ID, (byte, byte[])[] Headers, byte[] Body) : this(ID)
        {
            foreach ((byte header, byte[] content) in Headers)
                this.Headers[header] = content;

            this.Body = Body;
        }

        public T GetHeader<T>() where T : MessageHeader, new()
        {
            T header = new();
            if (!Headers.TryGetValue(header.HeaderID, out byte[] value))
                return null;

            header.Deserialize(value);
            return header;
        }

        public void Send()
        {
            List<byte> bytes = CreateHeader(ID).ToList();
            bytes[7] = (byte)Headers.Count;

            foreach (KeyValuePair<byte, byte[]> header in Headers)
            {
                bytes.Add(header.Key);

                ushort count = (ushort)header.Value.Length;

                if (count > ushort.MaxValue)
                    throw new Exception("Header too large (65k)");

                bytes.Add((byte)count);
                bytes.Add((byte)(count >> 8));
                bytes.AddRange(header.Value);
            }

            if (Body is not null)
                bytes.AddRange(Body);

            Send(bytes.ToArray());
        }
    }
}
