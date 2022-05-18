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

        public KeybindAttribute(string Path, params Key[] Keys) : base(Path) => this.Keys = Keys;

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

        public KeybindAttribute(string Path, T On, T Off, params Key[] Keys) : base(Path.EndsWith("Keybind") ? Path : Path + "Keybind", Off)
        {
            this.Keys = Keys;
            this.On = On;
            this.Off = Off;
        }

        public override void Setup(Type Type, MemberInfo minfo)
        {
            MinimalSetup(Type, minfo);

            base.DynamicValueChanged += _ => state ^= true;

            Managers.ModuleManager.Config.Bind(Section, Name, Keys).Register(() => Value = !state ? On : Off);
        }
    }
}
