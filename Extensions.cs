using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace KiraiMod.Core
{
    public static class Extensions
    {
        // System extensions
        public static EventHandler Invoke(this EventHandler handler)
        {
            handler(null, null);
            return handler;
        }

        // System.Type extensions
        public static void Initialize(this Type type) => System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(type.TypeHandle);
        public static void LogAs(this Type t, string name) => Plugin.Logger.Log(t is null ? BepInEx.Logging.LogLevel.Warning : BepInEx.Logging.LogLevel.Debug, $"{name}={t.Name}");

        // System.Reflection extensions
        public static HarmonyMethod ToHM(this MethodInfo minfo) => minfo is null ? null : new HarmonyMethod(minfo); 

        // UnityEngine extensions
        public static void Destroy(this Component component) => Object.Destroy(component);
        public static void Destroy(this Transform transform) => Object.Destroy(transform.gameObject);
        public static GameObject Instantiate(this GameObject go) => Object.Instantiate(go);
        public static GameObject DontDestroyOnLoad(this GameObject go)
        {
            UnityEngine.Object.DontDestroyOnLoad(go);
            return go;
        }

        // Coroutine extensions
        public static Coroutine Start(this IEnumerator enumerator) => Utils.Coroutine.Start(enumerator);
        public static void Stop(this Coroutine coroutine) => Utils.Coroutine.Stop(coroutine);

        // UnityEngine.Events extensions
        public static void On(this Toggle toggle, Action<bool> callback) => toggle.onValueChanged.AddListener(callback);
        public static void On(this Button button, Action callback) => button.onClick.AddListener(callback);
        public static void On(this Slider slider, Action<float> callback) => slider.onValueChanged.AddListener(callback);
        public static EventTrigger.Entry Setup(this EventTrigger.Entry entry, EventTriggerType type, UnityAction<BaseEventData> callback)
        {
            entry.eventID = type;
            entry.callback.AddListener(callback);
            return entry;
        }

        // Custom extensions
        public static void Register(this ConfigEntry<Key[]> entry, Action OnClick) => Managers.KeybindManager.RegisterKeybind(entry, OnClick);
    }
}
