using BepInEx.IL2CPP;
using System;
using System.Collections;
using System.Reflection;
using UnhollowerRuntimeLib;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using VRC.Core;

namespace KiraiMod.Core
{
    public static class Events
    {
        private const BindingFlags PrivateStatic = BindingFlags.NonPublic | BindingFlags.Static;
        private static readonly HarmonyLib.Harmony Harmony = new(Plugin.GUID);

        public static event Action ApplicationStart; // scene 0
        public static event Action UIManagerLoaded; // scene 1
        public static event Action<Scene> WorldLoaded; // scene -1
        public static event Action<Scene> WorldUnloaded; // scene -1
        public static event Action Update;
        public static event Action<ApiWorldInstance> WorldInstanceLoaded;

        [Obsolete("Use Events.Player.Joined instead")]
        public static event Action<Types.Player> PlayerJoined
        {
            add => Player.Joined += value;
            remove => Player.Joined -= value;
        }

        [Obsolete("Use Events.Player.Left instead")]
        public static event Action<Types.Player> PlayerLeft
        {
            add => Player.Left += value;
            remove => Player.Left -= value;
        }

        static Events()
        {
            SceneManager.add_sceneLoaded((UnityAction<Scene, LoadSceneMode>)HookSceneLoaded);
            SceneManager.add_sceneUnloaded((UnityAction<Scene>)HookSceneUnloaded);
            IL2CPPChainloader.AddUnityComponent<MonoHelper>();

            WorldLoaded += scene => WaitForInstance().Start();
        }

        private static void HookSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Plugin.Logger.LogInfo($"Loading scene {scene.buildIndex}: {scene.name}");

            if (scene.buildIndex == -1)
                WorldLoaded?.StableInvoke(scene);
            else if (scene.buildIndex == 0)
                ApplicationStart?.StableInvoke();
            else if (scene.buildIndex == 1)
                UIManagerLoaded?.StableInvoke();
        }

        private static void HookSceneUnloaded(Scene scene)
        {
            if (scene == null) return;
            if (scene.buildIndex == -1)
                WorldUnloaded?.StableInvoke(scene);
        }

        private static IEnumerator WaitForInstance()
        {
            ApiWorldInstance instance;
            while ((instance = Types.RoomManager.GetCurrentWorld()) == null)
                yield return null;

            WorldInstanceLoaded?.StableInvoke(instance);
        }

        public static class Player
        {
            public static event Action<Types.Player> Joined;
            public static event Action<Types.Player> Left;

            static Player()
            {
                Harmony.Patch(Types.Player.Type.GetMethod("OnNetworkReady"), null, typeof(Player).GetMethod(nameof(OnPlayerJoined), PrivateStatic).ToHM());
                Harmony.Patch(Types.Player.Type.GetMethod("OnDestroy"), typeof(Player).GetMethod(nameof(OnPlayerLeft), PrivateStatic).ToHM());
            }

            private static void OnPlayerJoined(MonoBehaviour __instance) => Joined?.StableInvoke(new Types.Player(__instance));
            private static void OnPlayerLeft(MonoBehaviour __instance) => Left?.StableInvoke(new Types.Player(__instance));
        }

        private class MonoHelper : MonoBehaviour
        {
            public MonoHelper() : base(ClassInjector.DerivedConstructorPointer<MonoHelper>()) => ClassInjector.DerivedConstructorBody(this);
            public MonoHelper(IntPtr ptr) : base(ptr) {}

            public void Update() => Events.Update?.StableInvoke();
        }
    }
}
