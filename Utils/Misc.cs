using System;

namespace KiraiMod.Core.Utils
{
    public static class Misc
    {
        [ThreadStatic]
        public static bool IsMainThread = true;
    }
}
