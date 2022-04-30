using System;
using System.Linq;
using System.Text;

namespace KiraiMod.Core
{
    public static class RPC
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

        public static void On(uint id, Action<Types.Player, byte[]> listener)
        {
            if (Managers.RPCManager.listeners.TryGetValue(id, out Action<Types.Player, byte[]> existing))
                Managers.RPCManager.listeners[id] = existing + listener;
            else Managers.RPCManager.listeners[id] = listener;
        }

        // the first 1000 ids are for Core (0-999)
        public enum CoreIDs : uint
        {
            AnnouncePresence = 0,
        }

        // todo: whitelist & blacklist system
        public enum TrustPolicy
        {
            Nobody,
            Friends,
            Everyone,
        }
    }
}
