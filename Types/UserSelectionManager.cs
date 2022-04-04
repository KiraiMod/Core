using System;
using System.Linq;
using System.Reflection;
using TypeScanner.Types;
using UnityEngine;
using VRC.Core;

namespace KiraiMod.Core.Types
{
    public static class UserSelectionManager
    {
        public static Type Type = ClassDef.Create(nameof(UserSelectionManager))
            .FromAssembly(Utils.Misc.AssemblyCSharp)
            .DerivesFrom<MonoBehaviour>()
            .ConstructorCount(2)
            .WithMethods(
                MethodDef.Create().WithName("Awake"),
                MethodDef.Create().WithName("OnDestroy"),
                MethodDef.Create().WithParameters(typeof(APIUser)),
                MethodDef.Create().WithName("FixedUpdate").ExpectAbsent(),
                MethodDef.Create().WithName("OnNetworkReady").ExpectAbsent()
            )
            .WithProperties(
                PropertyDef.Create().WithType<APIUser>()
            )
            .Setup().Resolved;

        public static PropertyInfo m_Instance = Type.GetProperties().FirstOrDefault(x => x.PropertyType == Type);
        public static MethodInfo m_SelectUser = Type.GetMethods().FirstOrDefault(x => {
            var parms = x.GetParameters();
            return parms.Length == 1 
            && parms[0].ParameterType == typeof(APIUser);
        });

        static UserSelectionManager()
        {
            Type.LogAs("UserSelectionManager");
            m_Instance.LogAs(".Instance");
            m_SelectUser.LogAs(".SelectUser");
        }

        public static Lazy<object> _Instance = new(() => m_Instance.GetValue(null));
        public static object Instance
        {
            get => _Instance.Value;
        }

        // todo: bake this to a delegate
        public static void SelectUser(APIUser user) => m_SelectUser.Invoke(Instance, new object[1] { user });
    }
}
