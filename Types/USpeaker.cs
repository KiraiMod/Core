using System;
using TypeScanner.Types;
using UnityEngine;

namespace KiraiMod.Core.Types
{
    public static class USpeaker
    {
        // this looks like a really fragile signature
        public static Type Type = ClassDef.Create("USpeaker")
            .FromAssembly(Events.Hooks.AssemblyCSharp)
            .DerivesFrom<MonoBehaviour>()
            .ConstructorCount(2)
            .WithMethods(
                MethodDef.Create().WithName("Awake"),
                MethodDef.Create().WithName("LateUpdate"),
                MethodDef.Create().WithName("OnDestroy"),
                MethodDef.Create().WithName("Start"),
                MethodDef.Create().WithName("Update")
            )
            .Setup()
            .Resolved;

        static USpeaker() => Type.LogAs(nameof(USpeaker));
    }
}
