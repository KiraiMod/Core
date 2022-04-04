using System;
using TypeScanner.Types;

namespace KiraiMod.Core.Types
{
    public static class UiVRCList
    {
        public static Type Type = ClassDef.Create(nameof(UiVRCList))
            .FromAssembly(Utils.Misc.AssemblyCSharp)
            .DerivesFrom<UnityEngine.MonoBehaviour>()
            .WithMethods(
                MethodDef.Create().WithName("ToggleExtend")
            )
            .Setup()
            .Resolved;

        static UiVRCList() => Type.LogAs(nameof(UiVRCList));
    }
}
