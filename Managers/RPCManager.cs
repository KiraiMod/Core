using BepInEx.Configuration;
using KiraiMod.Core.MessageAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using VRC.SDKBase;

namespace KiraiMod.Core.Managers
{
    // this needs to be rewritten
    // currently i do not think it should be needed to have an event id
    // it could be version specific instead
    // the bare minimum would just be a magic and a packet version
    public static class RPCManager
    {
        private static bool prevActive = false;
        public static bool Active
        {
            get => (!ModeratorSafety.Value || moderators <= 0) && (_forced || Enabled.Value);
        }

        private static bool _forced = false; // core will never set this
        public static bool ForceEnable //       it is intended for other plugins to use 
        { //                                    since they can run any code, it doesn't matter
            set //                              there is no reason to try to prevent it
            { //                                if you really wanted to prevent it, 
                _forced = true; //              you could increment the moderator count
                Refresh(); //                   and then turn moderator safety on
            }
        }

        public static Dictionary<uint, Action<Types.Player, Message>> listeners = new();

        public static ConfigEntry<bool> Enabled = Plugin.Configuration.Bind("RPC", "Enabled", true, "Should messages be exchanged between clients");
        public static ConfigEntry<bool> ModeratorSafety = Plugin.Configuration.Bind("RPC", "ModeratorSafety", false, "Should messages be dropped when a moderator is in the instance");

        private static int moderators = 0;

        static RPCManager()
        {
            Events.Player.Joined += player =>
            {
                if (player.VRCPlayerApi.isModerator) moderators++;
                if (player.VRCPlayerApi.isLocal) Message.Send((uint)CoreIDs.AnnouncePresence);
            };

            Events.Player.Left += player =>
            {
                if (player.VRCPlayerApi.isModerator)
                    moderators--;
            };

            Enabled.SettingChanged += ((EventHandler)((sender, args) => Refresh())).Invoke();
        }

        public static void Refresh()
        {
            bool active = Active;
            if (active == prevActive)
                return;

            prevActive = active;

            if (active)
                Events.VRCEvent.Recieved += OnEvent;
            else Events.VRCEvent.Recieved -= OnEvent;
        }

        private static void OnEvent(Types.Player player, ref VRC_EventHandler.VrcEvent ev, ref VRC_EventHandler.VrcBroadcastType type)
        {
            if (ev.ParameterString == "UdonSyncRunProgramAsRPC")
                HandleKMEv(player, ev);
        }

        // message header
        // 0x53 0x06            short   magic
        // 0x?? 0x?? 0x?? 0x??  int     event id
        // 0x??                 byte    reserved
        // 0x??                 byte    amount of headers
        private static unsafe void HandleKMEv(Types.Player sender, VRC_EventHandler.VrcEvent ev)
        {
            Message? _kmev = ParseEventData(ev.ParameterBytes);

            if (_kmev is null) return;
            Message kmev = (Message)_kmev;

            Plugin.Logger.LogDebug($"KMEvent {kmev.ID} ({sender.VRCPlayerApi.displayName})");
            foreach (KeyValuePair<byte, byte[]> pair in kmev.Headers)
                Plugin.Logger.LogDebug($"\t{pair.Key.ToString("X2")}: {string.Join(",", pair.Value.Select(x => x.ToString("X2")))}");
            if (kmev.Body.Length > 0)
                Plugin.Logger.LogDebug(string.Join(",", kmev.Body.Select(x => x.ToString("X2"))));

            if (listeners.TryGetValue(kmev.ID, out Action<Types.Player, Message> listener))
                listener?.StableInvoke(sender, kmev);
        }

        private static unsafe Message? ParseEventData(UnhollowerBaseLib.Il2CppStructArray<byte> data)
        {
            byte* ptr = (byte*)data.Pointer + 32;

            // size check
            if (data.Length < 8)
                return null;

            // magic check
            if ((*(short*)ptr) != 0x06_53) // reversed due to little endian
                return null;

            uint id = *(uint*)(ptr + 2);
            byte headerCount = *(ptr + 7);

            Message msg = new(id);

            int offset = 8;
            byte[] dataArray = data.ToArray();

            for (byte i = 0; i < headerCount; i++)
            {
                int sizeRemaining = data.Length - offset - 2;
                if (sizeRemaining < 0) return null;

                byte* headerPtr = ptr + offset;

                byte headerID = *headerPtr;
                ushort headerSize = *(ushort*)(headerPtr + 1);

                if (sizeRemaining - headerSize < 0) return null;

                offset += headerSize + 3;

                msg.Headers[headerID] = dataArray[(offset - headerSize)..offset];
            }

            msg.Body = dataArray[offset..];

            return msg;
        }
    }
}
