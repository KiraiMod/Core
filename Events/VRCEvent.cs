using System;
using System.Linq;
using UnityEngine;
using VRC.SDKBase;

namespace KiraiMod.Core
{
    partial class Events
    {
        public static class VRCEvent
        {
            public delegate void OnVRCEvent_T(Types.Player player, ref VRC_EventHandler.VrcEvent ev, ref VRC_EventHandler.VrcBroadcastType broadcast);

            public static event OnVRCEvent_T Recieved;

            static VRCEvent()
            {
                HarmonyLib.HarmonyMethod hk = typeof(VRCEvent).GetMethod(nameof(OnVRCEvent), PrivateStatic).ToHM();

                Types.VRC_EventDispatcher.Type
                    .GetMethods()
                    .Where(x =>
                    {
                        if (x.ReturnType != typeof(void))
                            return false;

                        var parms = x.GetParameters();
                        return parms.Length == 5
                            && parms[0].ParameterType == Types.Player.Type
                            && parms[1].ParameterType == typeof(VRC_EventHandler.VrcEvent)
                            && parms[2].ParameterType == typeof(VRC_EventHandler.VrcBroadcastType)
                            && parms[3].ParameterType == typeof(int)
                            && parms[4].ParameterType == typeof(float);
                    })
                    .ForEach(x => Harmony.Patch(x, hk));
            }

            private static void OnVRCEvent(MonoBehaviour __0, ref VRC_EventHandler.VrcEvent __1, ref VRC_EventHandler.VrcBroadcastType __2)
            {
                Types.Player player = new(__0);
                Delegate[] list = Recieved.GetInvocationList();

                for (int i = 0; i < list.Length; i++)
                    try { (list[i] as OnVRCEvent_T)(player, ref __1, ref __2); }
                    catch (Exception ex) { Plugin.Logger.LogError(ex); }
            }
        }
    }
}
