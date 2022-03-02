﻿using BepInEx.IL2CPP;
using System;
using System.Linq;
using System.Reflection;
using UnhollowerRuntimeLib;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace KiraiMod.Core
{
    public static class Events
    {
        public static event Action ApplicationStart; // scene 0
        public static event Action UIManagerLoaded; // scene 1
        public static event Action<Scene> WorldLoaded; // scene -1
        public static event Action Update;
        public static event Action<Types.Player> PlayerJoined;
        public static event Action<Types.Player> PlayerLeft;

        static Events()
        {
            SceneManager.add_sceneLoaded((UnityAction<Scene, LoadSceneMode>)HookSceneLoaded);
            IL2CPPChainloader.AddUnityComponent<MonoHelper>();

            typeof(Hooks).Initialize();
        }

        public static void HookSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Plugin.Logger.LogInfo($"Loading scene {scene.buildIndex}: {scene.name}");

            if (scene.buildIndex == -1)
                WorldLoaded?.Invoke(scene);
            else if (scene.buildIndex == 0)
                ApplicationStart?.Invoke();
            else if (scene.buildIndex == 1)
                UIManagerLoaded?.Invoke();
        }

        internal static class Hooks
        {
            public const BindingFlags PrivateStatic = BindingFlags.NonPublic | BindingFlags.Static;

            public static Assembly AssemblyCSharp = AppDomain.CurrentDomain.GetAssemblies().First(f => f.GetName().Name == "Assembly-CSharp");
            public static HarmonyLib.Harmony Harmony = new(Plugin.GUID);

            static Hooks()
            {
                Harmony.Patch(Types.Player.Type.GetMethod("OnNetworkReady"), null, typeof(Hooks).GetMethod(nameof(OnPlayerJoined), PrivateStatic).ToHM());
                Harmony.Patch(Types.Player.Type.GetMethod("OnDestroy"), typeof(Hooks).GetMethod(nameof(OnPlayerLeft), PrivateStatic).ToHM());
            }

            private static void OnPlayerJoined(MonoBehaviour __instance) => PlayerJoined?.Invoke(new Types.Player(__instance));
            private static void OnPlayerLeft(MonoBehaviour __instance) => PlayerLeft?.Invoke(new Types.Player(__instance));
        }

        private class MonoHelper : MonoBehaviour
        {
            public MonoHelper() : base(ClassInjector.DerivedConstructorPointer<MonoHelper>()) => ClassInjector.DerivedConstructorBody(this);
            public MonoHelper(IntPtr ptr) : base(ptr) {}

            public void Update() => Events.Update?.Invoke();
        }
    }
}
