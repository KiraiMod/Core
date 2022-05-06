using System;

namespace KiraiMod.Core.ModuleAPI
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ModuleAttribute : Attribute 
    {
        internal Type Type;
    }
}
