using System;
using System.Collections;
using UnityEngine.SceneManagement;
using VRC.Core;

namespace KiraiMod.Core
{
    partial class Events
    {
        public static class World
        {
            public static event Action<Scene> Loaded; // scene -1
            public static event Action<Scene> Unloaded; // scene -1
            public static event Action<ApiWorldInstance> InstanceLoaded;

            static World()
            {
                SceneLoaded += scene =>
                {
                    WaitForInstance().Start();

                    if (scene.buildIndex == -1)
                        Loaded?.StableInvoke(scene);
                };

                SceneUnloaded += scene =>
                {
                    if (scene.buildIndex != -1)
                        Unloaded?.StableInvoke(scene);
                };
            }

            private static IEnumerator WaitForInstance()
            {
                ApiWorldInstance instance;
                while ((instance = Types.RoomManager.GetCurrentWorld()) == null)
                    yield return null;

                InstanceLoaded?.StableInvoke(instance);
            }
        }
    }
}
