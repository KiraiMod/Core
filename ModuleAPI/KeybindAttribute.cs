using System;
using System.Reflection;
using UnityEngine.InputSystem;

namespace KiraiMod.Core.ModuleAPI
{
    // todo: support boolean fields/props?
    [AttributeUsage(AttributeTargets.Method)]
    public class KeybindAttribute : InteractAttribute
    {
        private readonly Key[] Keys;

        public KeybindAttribute(string Name, params Key[] Keys) : this(null, Name, Keys) {}
        public KeybindAttribute(string Section, string Name, params Key[] Keys) : base(Section, Name + "Keybind") => this.Keys = Keys;

        public override void Setup(Type Type, MemberInfo minfo)
        {
            base.Setup(Type, minfo);
            Plugin.Configuration.Bind(Section, Name, Keys).Register(Invoke);
        }
    }
}
