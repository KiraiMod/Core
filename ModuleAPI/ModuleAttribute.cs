using System;

namespace KiraiMod.Core.ModuleAPI
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ModuleAttribute : Attribute
    {
        public Type __declarer;
    }
}
