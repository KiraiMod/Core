using System;
using System.Reflection;

namespace KiraiMod.Core.ModuleAPI
{
    public abstract class MemberAttribute : Attribute
    {
        public Type Type;
        public string Section;
        public string Name;

        public MemberAttribute(string Section, string Name)
        {
            this.Section = Section;
            this.Name = Name;
        }

        public virtual void Setup(Type Type, MemberInfo minfo) => this.Type = Type;
    }
}
