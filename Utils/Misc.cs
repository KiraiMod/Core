using System;
using System.Linq;
using System.Reflection;

namespace KiraiMod.Core.Utils
{
    public static class Misc
    {
        [ThreadStatic]
        public static bool IsMainThread = true;

        public static Assembly AssemblyCSharp = AppDomain.CurrentDomain.GetAssemblies().First(x => x.GetName().Name == "Assembly-CSharp");
    }
}
