using System;
using System.Reflection;
using UnityEngine.InputSystem;

namespace KiraiMod.Core.ModuleAPI
{
    // when the bind manager is created, this will be deprecated
    [AttributeUsage(AttributeTargets.Method)]
    public class KeybindAttribute : InteractAttribute
    {
        private readonly Key[] Keys;

        public KeybindAttribute(string Name, params Key[] Keys) : this(null, Name, Keys) {}
        public KeybindAttribute(string Section, string Name, params Key[] Keys) : base(Section, Name + "Keybind") => this.Keys = Keys;

        public override void Setup(Type Type, MemberInfo minfo)
        {
            base.Setup(Type, minfo);
            Managers.ModuleManager.Config.Bind(Section, Name, Keys).Register(Invoke);
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class KeybindAttribute<T> : ConfigureAttribute<T>
    {
        private readonly Key[] Keys;
        private readonly T On;
        private readonly T Off;
        private bool state;

        public KeybindAttribute(string Name, T On, T Off, params Key[] Keys) : this(null, Name, On, Off, Keys) { }
        public KeybindAttribute(string Section, string Name, T On, T Off, params Key[] Keys) : base(Section, Name + "Keybind", Off)
        {
            this.Keys = Keys;
            this.On = On;
            this.Off = Off;
        }

        public override void Setup(Type Type, MemberInfo minfo)
        {
            base.Setup(Type, minfo);
            Managers.ModuleManager.Config.Bind(Section, Name, Keys).Register(() => Value = (state ^= true) ? On : Off);
        }
    }
}
