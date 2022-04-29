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
    public static partial class Events
    {
        private const BindingFlags PrivateStatic = BindingFlags.NonPublic | BindingFlags.Static;
        private static readonly HarmonyLib.Harmony Harmony = new(Plugin.GUID);

        public static event Action<Scene> SceneLoaded;
        public static event Action<Scene> SceneUnloaded;

        public static event Action ApplicationStart; // scene 0
        public static event Action UIManagerLoaded; // scene 1
        public static event Action EmptyLoaded; // scene 2

        [Obsolete("Use Events.World.Loaded instead")]
        public static event Action<Scene> WorldLoaded
        {
            add => World.Loaded += value;
            remove => World.Loaded -= value;
        }

        [Obsolete("Use Events.World.Loaded instead")]
        public static event Action<Scene> WorldUnloaded
        {
            add => World.Loaded += value;
            remove => World.Loaded -= value;
        }

        [Obsolete("Use Events.World.Loaded instead")]
        public static event Action<ApiWorldInstance> WorldInstanceLoaded
        {
            add => World.InstanceLoaded += value;
            remove => World.InstanceLoaded -= value;
        }

        public static event Action Update;

        [Obsolete("Use Events.Player.Joined instead", true)]
        public static event Action<Types.Player> PlayerJoined
        {
            add => Player.Joined += value;
            remove => Player.Joined -= value;
        }

        [Obsolete("Use Events.Player.Left instead", true)]
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
        }

        private static void HookSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Plugin.Logger.LogInfo($"Loaded scene {scene.buildIndex}: {scene.name}");

            SceneLoaded?.StableInvoke(scene);

            if (scene.buildIndex == 0)
                ApplicationStart?.StableInvoke();
            else if (scene.buildIndex == 1)
                UIManagerLoaded?.StableInvoke();
            else if (scene.buildIndex == 2)
                EmptyLoaded?.StableInvoke();
        }

        private static void HookSceneUnloaded(Scene scene)
        {
            if (scene == null) return;
            SceneUnloaded?.StableInvoke(scene);
        }

        private class MonoHelper : MonoBehaviour
        {
            public MonoHelper() : base(ClassInjector.DerivedConstructorPointer<MonoHelper>()) => ClassInjector.DerivedConstructorBody(this);
            public MonoHelper(IntPtr ptr) : base(ptr) {}

            public void Update() => Events.Update?.StableInvoke();
        }
    }
}
