using ExitGames.Client.Photon;
using System;
using System.Linq;
using System.Reflection;

namespace KiraiMod.Core.Types
{
    public static class VRCNetworkingClient
    {
        public readonly static Type Type;
        public readonly static MethodInfo m_OnEvent;
        public readonly static MethodInfo m_OpRaiseEvent;

        static VRCNetworkingClient()
        {
            m_OnEvent = Events.Hooks.AssemblyCSharp.GetExportedTypes()
                .Where(x => !x.IsGenericType && x.DeclaringType == null && x.BaseType?.BaseType == typeof(Il2CppSystem.Object))
                .SelectMany(x => x.GetMethods())
                .FirstOrDefault(x =>
                {
                    var ps = x.GetParameters();
                    return x.ReturnType == typeof(void)
                    && ps.Length == 1
                    && ps[0].ParameterType == typeof(EventData)
                    && x.Name == "OnEvent";
                });

            Type = m_OnEvent.DeclaringType;

            m_OpRaiseEvent = Type.GetMethods()
                .Where(x =>
                {
                    ParameterInfo[] args = x.GetParameters();
                    return args.Length == 4
                    && args[0].ParameterType == typeof(byte)
                    && args[1].ParameterType == typeof(Il2CppSystem.Object)
                    //&& !args[2].ParameterType.IsValueType
                    && args[3].ParameterType == typeof(SendOptions);
                }).ElementAt(0);

            Type.LogAs("VRCNetworkingClient");
            m_OnEvent.LogAs(".OnEvent");
            m_OpRaiseEvent.LogAs(".OpRaiseEvent");
        }
    }
}
