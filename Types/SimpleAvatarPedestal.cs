using System;
using TypeScanner.Types;
using UnityEngine;

namespace KiraiMod.Core.Types
{
    public static class SimpleAvatarPedestal
    {
        public static Type Type = ClassDef.Create(nameof(SimpleAvatarPedestal))
            .FromAssembly(Utils.Misc.AssemblyCSharp)
            .DerivesFrom<MonoBehaviour>()
            .WithMethods(
                MethodDef.Create().WithName("LateUpdate"),
                MethodDef.Create().WithName("OnNetworkReady").ExpectAbsent(),
                MethodDef.Create().WithParameters(typeof(VRC.Core.ApiAvatar))
            )
            .WithProperties(
                PropertyDef.Create().WithType<VRC.Core.ApiAvatar>()
            )
            .Setup()
            .Resolved;

        static SimpleAvatarPedestal() => Type.LogAs(nameof(SimpleAvatarPedestal));
    }
}
