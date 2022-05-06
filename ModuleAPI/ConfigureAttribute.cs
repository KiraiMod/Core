using BepInEx.Configuration;
using System;
using System.Reflection;

namespace KiraiMod.Core.ModuleAPI
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ConfigureAttribute<T> : MemberAttribute
    {
        public T Value
        {
            get => Getter();
            set {
                T val = Getter();
                if ((value is null && val == null) || value.Equals(val))
                    return;
                Setter(Entry.Value = value);
            }
        }

        private ConfigEntry<T> Entry;
        private Action<T> Setter;
        private Func<T> Getter;
        private readonly T Default;

        public ConfigureAttribute(string Section, string Name, T Default) : base(Section, Name) => this.Default = Default;

        public override void Setup(Type Type, MemberInfo minfo)
        {
            base.Setup(Type, minfo);

            if (minfo is PropertyInfo prop)
            {
                MethodInfo _setter = prop.GetSetMethod();
                if (_setter is not null)
                    Setter = (Action<T>)Delegate.CreateDelegate(typeof(Action<T>), _setter);
                else Setter = (T val) => { };

                MethodInfo _getter = prop.GetGetMethod();
                if (_getter is not null)
                    Getter = (Func<T>)Delegate.CreateDelegate(typeof(Func<T>), _getter);
                else Getter = () => default;
            }
            else if (minfo is FieldInfo field)
            {
                Setter = value => field.SetValue(null, value);
                Getter = () => (T)field.GetValue(null);
            }

            Entry = Managers.ModuleManager.Config.Bind(Section, Name, Default);
            Entry.SettingChanged += ((EventHandler)((sender, args) => Value = Entry.Value)).Invoke();
        }
    }
}
