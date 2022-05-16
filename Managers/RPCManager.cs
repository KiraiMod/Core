using BepInEx.Configuration;
using KiraiMod.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
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

        public static Dictionary<uint, Action<Types.Player, byte[]>> listeners = new();

        public static ConfigEntry<bool> Enabled = Plugin.Configuration.Bind("RPC", "Enabled", true, "Should messages be exchanged between clients");
        public static ConfigEntry<bool> ModeratorSafety = Plugin.Configuration.Bind("RPC", "ModeratorSafety", false, "Should messages be dropped when a moderator is in the instance");

        private static int moderators = 0;

        static RPCManager()
        {
            Events.Player.Joined += player =>
            {
                if (player.VRCPlayerApi.isModerator) moderators++;
                if (player.VRCPlayerApi.isLocal) RPC.Send((uint)RPC.CoreIDs.AnnouncePresence);
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

        //  KMEv header
        // 0x53 0x06            short   magic
        // 0x?? 0x?? 0x?? 0x??  int     event id
        // 0x??                 byte    packet version
        // 0x??                 byte    reserved
        private static unsafe void HandleKMEv(Types.Player sender, VRC_EventHandler.VrcEvent ev)
        {
            if (ev.ParameterBytes.Length < 8)
                return;

            byte* ptr = (byte*)ev.ParameterBytes.Pointer + 32;

            if ((*(short*)ptr) != 0x06_53) // reversed due to little endian
                return;

            uint id = *(uint*)(ptr + 2);
            byte reserved = *(ptr + 7);

            switch (*(ptr + 6))
            {
                case 0:
                    HandleKMEv0(sender, id, ev.ParameterBytes.Skip(8).ToArray()); // needs a better way to skip 
                    break;

                default:
                    break;
            }
        }

        private static void HandleKMEv0(Types.Player sender, uint id, byte[] data)
        {
            Plugin.Logger.LogDebug($"KMEv0 {id} ({sender.VRCPlayerApi.displayName}): {string.Concat(data.Select(x => x.ToString("X")))}");

            if (listeners.TryGetValue(id, out Action<Types.Player, byte[]> listener))
                listener?.StableInvoke(sender, data);
        }
    }
}
