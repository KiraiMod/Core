using System;
using TypeScanner.Types;

namespace KiraiMod.Core.Types
{
    public static class VRC_EventDispatcher
    {
        public static Type Type = ClassDef.Create(nameof(VRC_EventDispatcher))
            .FromAssembly(Utils.Misc.AssemblyCSharp)
            .DerivesFrom<VRC.SDKBase.VRC_EventDispatcher>()
            .Setup()
            .Resolved;

        static VRC_EventDispatcher() => Type.LogAs(nameof(VRC_EventDispatcher));
    }
}
