using System;
using System.Collections.Generic;
using System.Text;
using TypeScanner.Types;

namespace KiraiMod.Core.Types
{
    public static class UiAvatarList
    {
        public static Type Type = ClassDef.Create("UiAvatarList")
            .FromAssembly(Events.Hooks.AssemblyCSharp)
            .DerivesFrom(UiVRCList.Type)
            .Setup()
            .Resolved;

        static UiAvatarList() => Type.LogAs(nameof(UiAvatarList));
    }
}
