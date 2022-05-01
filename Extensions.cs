using BepInEx.Configuration;
using HarmonyLib;
using KiraiMod.Core.Utils;
using System;
using System.Collections;
using System.Linq;
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

        public static void ForEach<T>(this System.Collections.Generic.IEnumerable<T> arr, Action<T> func) { foreach (T elem in arr) func(elem); }
        public static T Pop<T>(this System.Collections.Generic.List<T> arr) { T elem = arr[^1]; arr.RemoveAt(arr.Count - 1); return elem; }

        public static void StableInvoke(this Action action) => action.GetInvocationList().Cast<Action>().ForEach(sub => { try { sub(); } catch (Exception ex) { Plugin.Logger.LogError(ex); } });
        public static void StableInvoke<T>(this Action<T> action, T value) => action.GetInvocationList().Cast<Action<T>>().ForEach(sub => { try { sub(value); } catch (Exception ex) { Plugin.Logger.LogError(ex); } });
        public static void StableInvoke<T1,T2>(this Action<T1,T2> action, T1 value1, T2 value2) => action.GetInvocationList().Cast<Action<T1,T2>>().ForEach(sub => { try { sub(value1, value2); } catch (Exception ex) { Plugin.Logger.LogError(ex); } });
        public static void StableInvoke<T1,T2,T3>(this Action<T1,T2,T3> action, T1 value1, T2 value2, T3 value3) => action.GetInvocationList().Cast<Action<T1,T2,T3>>().ForEach(sub => { try { sub(value1, value2, value3); } catch (Exception ex) { Plugin.Logger.LogError(ex); } });

        // System.Type extensions
        public static void Initialize(this Type type) => System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(type.TypeHandle);
        public static void LogAs(this Type t, string name) => Plugin.Logger.Log(t == null ? BepInEx.Logging.LogLevel.Warning : BepInEx.Logging.LogLevel.Debug, $"{name}={t?.Name}");
        public static void LogAs(this MemberInfo t, string name) => Plugin.Logger.Log(t == null ? BepInEx.Logging.LogLevel.Warning : BepInEx.Logging.LogLevel.Debug, $"{name}={t?.Name}");
        public static PropertyInfo Singleton(this Type t) => t.GetProperties().FirstOrDefault(x => x.PropertyType == t);

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

        public static Vector2 AddX(this Vector2 vec, float value) { vec.x += value; return vec; }
        public static Vector2 AddY(this Vector2 vec, float value) { vec.y += value; return vec; }
        public static Vector3 AddX(this Vector3 vec, float value) { vec.x += value; return vec; }
        public static Vector3 AddY(this Vector3 vec, float value) { vec.y += value; return vec; }
        public static Vector3 AddZ(this Vector3 vec, float value) { vec.z += value; return vec; }

        // Coroutine extensions
        public static UnityEngine.Coroutine Start(this IEnumerator enumerator) => Utils.Coroutine.Start(enumerator);
        public static void Stop(this UnityEngine.Coroutine coroutine) => Utils.Coroutine.Stop(coroutine);

        // UnityEngine.Events extensions
        public static Toggle On(this Toggle toggle, Action<bool> callback) { toggle.onValueChanged.AddListener(callback); return toggle; }
        public static Button On(this Button button, Action callback) { button.onClick.AddListener(callback); return button; }
        public static Slider On(this Slider slider, Action<float> callback) { slider.onValueChanged.AddListener(callback); return slider; }
        public static EventTrigger.Entry Setup(this EventTrigger.Entry entry, EventTriggerType type, UnityAction<BaseEventData> callback)
        {
            entry.eventID = type;
            entry.callback.AddListener(callback);
            return entry;
        }

        // Custom extensions
        public static void Register(this ConfigEntry<Key[]> entry, Action OnClick) => Managers.KeybindManager.RegisterKeybind(entry, OnClick);

        // Utils.Bound
        public static Bound<bool> Bind(this Bound<bool> bound, Toggle component)
        {
            component.On(value => bound.Value = value);
            bound.ValueChanged += value => component.Set(value, false);
            return bound;
        }
    }
}
