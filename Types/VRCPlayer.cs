using Il2CppSystem.Collections;
using System;
using System.Linq;
using System.Reflection;

namespace KiraiMod.Core.Types
{
    public static class VRCPlayer
    {
        public static readonly Type Type = Player.Type.GetProperties()
            .First(x => x.PropertyType?.BaseType == typeof(Il2CppSystem.Object) && x.PropertyType.Name.EndsWith("Unique"))
            .PropertyType;

        public static readonly PropertyInfo m_GetHashtable = Type.GetProperties()
            .FirstOrDefault(x => x.PropertyType == typeof(Hashtable));

        static VRCPlayer()
        {
            Type.LogAs("VRCPlayer");
            m_GetHashtable.LogAs(".Properties");
        }
    }
}
