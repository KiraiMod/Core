﻿using BepInEx.Configuration;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace KiraiMod.Core
{
    public static class Extensions
    {
        public static void Initialize(this Type type) => System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(type.TypeHandle);

        public static void Destroy(this Component component) => Object.Destroy(component);
        public static void Destroy(this Transform transform) => Object.Destroy(transform.gameObject);

        public static Coroutine Start(this IEnumerator enumerator) => Utils.Coroutine.Start(enumerator);
        public static void Stop(this Coroutine coroutine) => Utils.Coroutine.Stop(coroutine);

        public static GameObject Instantiate(this GameObject go) => Object.Instantiate(go);

        public static GameObject DontDestroyOnLoad(this GameObject go)
        {
            UnityEngine.Object.DontDestroyOnLoad(go);
            return go;
        }

        public static EventTrigger.Entry Setup(this EventTrigger.Entry entry, EventTriggerType type, UnityAction<BaseEventData> callback)
        {
            entry.eventID = type;
            entry.callback.AddListener(callback);
            return entry;
        }

        public static EventHandler Invoke(this EventHandler handler)
        {
            handler(null, null);
            return handler;
        }

        public static void On(this Toggle toggle, Action<bool> callback) => toggle.onValueChanged.AddListener(callback);
        public static void On(this Button button, Action callback) => button.onClick.AddListener(callback);
        public static void On(this Slider slider, Action<float> callback) => slider.onValueChanged.AddListener(callback);

        public static void Register(this ConfigEntry<Key[]> entry, Action OnClick) => Managers.KeybindManager.RegisterKeybind(entry, OnClick);
    }
}
