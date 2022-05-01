using System;

namespace KiraiMod.Core
{
    partial class Events
    {
        public static class Portal
        {
            public static event Action<VRC.Core.ApiWorldInstance> Configure;

            static Portal() =>
                Harmony.Patch(
                    Types.PortalInternal.m_CreatePortal,
                    null,
                    typeof(Portal).GetMethod(nameof(OnConfigurePortalPost), PrivateStatic).ToHM()
                );

            private static void OnConfigurePortalPost(VRC.Core.ApiWorldInstance __1) => Configure?.StableInvoke(__1);
        }
    }
}
