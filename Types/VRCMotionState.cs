using System;
using TypeScanner.Types;
using UnityEngine;

namespace KiraiMod.Core.Types
{
    public static class VRCMotionState
    {
        public static Type Type = ClassDef.Create("VRCMotionState")
            .FromAssembly(Utils.Misc.AssemblyCSharp)
            .DerivesFrom<MonoBehaviour>()
            .WithMethods(
                MethodDef.Create().WithName("Awake"),
                MethodDef.Create().WithName("Reset"),
                MethodDef.Create().WithName("Start"),
                MethodDef.Create().WithName("OnControllerColliderHit")
            )
            .Setup()
            .Resolved;

        static VRCMotionState() => Type.LogAs("VRCMotionState");
    }
}
