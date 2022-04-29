using System;
using UnityEngine;

namespace KiraiMod.Core
{
    partial class Events
    {
        public static class Player
        {
            public static event Action<Types.Player> Joined;
            public static event Action<Types.Player> Left;

            static Player()
            {
                Harmony.Patch(Types.Player.Type.GetMethod("OnNetworkReady"), null, typeof(Player).GetMethod(nameof(OnPlayerJoined), PrivateStatic).ToHM());
                Harmony.Patch(Types.Player.Type.GetMethod("OnDestroy"), typeof(Player).GetMethod(nameof(OnPlayerLeft), PrivateStatic).ToHM());
            }

            private static void OnPlayerJoined(MonoBehaviour __instance) => Joined?.StableInvoke(new Types.Player(__instance));
            private static void OnPlayerLeft(MonoBehaviour __instance) => Left?.StableInvoke(new Types.Player(__instance));
        }

    }
}
