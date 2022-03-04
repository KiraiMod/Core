using KiraiMod.Core;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using VRC.Core;

namespace KiraiMod.Core.Types
{
    public static class RoomManager
    {
        public static PropertyInfo m_CurrentWorld = AppDomain.CurrentDomain.GetAssemblies()
               .First(f => f.GetName().Name == "Assembly-CSharp")
               .GetExportedTypes()
               .Where(x => x.BaseType == typeof(MonoBehaviour))
               .Where(x => x.GetMethod("OnConnectedToMaster") != null)
               .SelectMany(x => x.GetProperties(BindingFlags.Public | BindingFlags.Static))
               .FirstOrDefault(x => x.PropertyType == typeof(ApiWorldInstance));

        static RoomManager() => m_CurrentWorld.LogAs("RoomManager");

        public static ApiWorldInstance GetCurrentWorld() => (ApiWorldInstance)m_CurrentWorld.GetValue(null);
    }
}
