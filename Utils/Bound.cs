using BepInEx.Configuration;
using System;

namespace KiraiMod.Core.Utils
{
    public class Bound<T>
    {
        public event Action<T> ValueChanged;

        public Bound() { }
        public Bound(T value) => _value = value;

        public T _value;
        public T Value
        {
            [Obsolete("Please read from _value directly")]
            get => _value;
            set {
                if (value?.Equals(_value) ?? _value is null) return;
                _value = value;

                ValueChanged?.Invoke(value);
            }
        }

        public Bound<T> Bind(ConfigEntry<T> entry)
        {
            entry.SettingChanged += ((EventHandler)((sender, args) => _value = entry.Value)).Invoke();
            ValueChanged += value => entry.Value = value;
            return this;
        }

        public Bound<T> Set(T value)
        {
            Value = value;
            return this;
        }

        [Obsolete("Please read from _value directly")]
        public static implicit operator T(Bound<T> bound) => bound._value;
    }
}
