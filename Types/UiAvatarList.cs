using System;
using TypeScanner.Types;

namespace KiraiMod.Core.Types
{
    public static class UiAvatarList
    {
        public static Type Type = ClassDef.Create("UiAvatarList")
            .FromAssembly(Utils.Misc.AssemblyCSharp)
            .DerivesFrom(UiVRCList.Type)
            .Setup()
            .Resolved;

        static UiAvatarList() => Type.LogAs(nameof(UiAvatarList));
    }
}
