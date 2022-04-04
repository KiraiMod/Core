using System;
using TypeScanner.Types;

namespace KiraiMod.Core.Types
{
    public static class DownloadManager
    {
        public static Type Type = ClassDef.Create(nameof(DownloadManager))
            .FromAssembly(Utils.Misc.AssemblyCSharp)
            .WithProperties(
                PropertyDef.Create().WithType<UnityEngine.Cache>()
            )
            .Setup()
            .Resolved;

        static DownloadManager() => Type.LogAs(nameof(DownloadManager));
    }
}
