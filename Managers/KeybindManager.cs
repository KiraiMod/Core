﻿using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace KiraiMod.Core.Managers
{
    // this only supports the keyboard
    // once unhollower/interop supports blittable structs in delegates
    // i will replace this with a better bind system
    public static class KeybindManager
    {
        public static readonly Dictionary<string, Keybind> binds = new();

        static KeybindManager()
        {
            TomlTypeConverter.AddConverter(typeof(Key[]), new TypeConverter()
            {
                ConvertToObject = (str, _) => str[1..^1].Split(',').Select(x => (Key)Enum.Parse(typeof(Key), x.Trim())).ToArray(),
                ConvertToString = (obj, _) => $"[{string.Join(", ", (obj as Key[]).Select(x => Enum.GetName(typeof(Key), x)))}]",
            });

            InputSystem.onEvent += (Il2CppSystem.Action<InputEventPtr, InputDevice>)OnEvent;
        }

        // this will always run less than OnUpdate would've
        // makes more sense to only check keybinds when keys are modified
        private static void OnEvent(InputEventPtr ptr, InputDevice dev)
        {
            Keyboard kb = dev.TryCast<Keyboard>();
            if (kb == null) return;

            foreach (Keybind bind in binds.Values)
            {
                int kc = 0;
                foreach (Key key in bind.keys)
                    if (kb[key].isPressed)
                        kc++;

                if (bind.previous)
                {
                    if (kc != bind.keys.Length)
                        bind.previous = false;
                }
                else if (kc == bind.keys.Length)
                {
                    bind.previous = true;
                    bind.OnClick();
                }
            }
        }

        internal static void RegisterKeybind(ConfigEntry<Key[]> entry, Action OnClick)
        {
            Keybind bind = new()
            {
                key = $"{entry.Definition.Section}::{entry.Definition.Key}",
                keys = entry.Value,
                OnClick = OnClick
            };

            entry.SettingChanged += (sender, args) => bind.keys = entry.Value;

            binds[bind.key] = bind;
        }

        public class Keybind
        {
            public string key;
            public bool previous;
            public Key[] keys;
            public Action OnClick;
        }
    }
}
