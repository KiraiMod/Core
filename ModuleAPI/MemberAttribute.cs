using System;
using System.Collections.Generic;
using System.Reflection;

namespace KiraiMod.Core.ModuleAPI
{
    public abstract class MemberAttribute : Attribute
    {
        public static event Action<MemberAttribute> Added;
        public static List<MemberAttribute> All = new();

        public Type Type;
        public string Section;
        public string Name;

        public MemberAttribute(string Section, string Name)
        {
            this.Section = Section;
            this.Name = Name;
        }

        public virtual void Setup(Type Type, MemberInfo minfo) => this.Type = Type;
        internal void SetupInternal(Type Type, MemberInfo minfo)
        {
            Setup(Type, minfo);
            All.Add(this);
            Added?.StableInvoke(this);
        }
    }
}
