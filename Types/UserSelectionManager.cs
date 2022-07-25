using System;
using System.Linq;
using System.Reflection;
using TypeScanner.Types;
using UnityEngine;
using VRC.Core;
using VRC.SDKBase;

namespace KiraiMod.Core.Types
{
    public static class UserSelectionManager
    {
        public static readonly Type Type = ClassDef.Create(nameof(UserSelectionManager))
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

        private static readonly PropertyInfo[] m_APIUsers = Type.GetProperties().Where(x => x.PropertyType == typeof(APIUser)).ToArray();
        private static readonly PropertyInfo m_Instance = Type.GetProperties().FirstOrDefault(x => x.PropertyType == Type);
        private static readonly MethodInfo m_SelectUser = Type.GetMethods().FirstOrDefault(x => {
            ParameterInfo[] parms = x.GetParameters();
            return parms.Length == 1
                && parms[0].ParameterType == typeof(APIUser);
        });

        static UserSelectionManager()
        {
            Type.LogAs("UserSelectionManager");
            m_Instance.LogAs(".Instance");
            m_SelectUser.LogAs(".SelectUser");
        }

        public static readonly Lazy<object> _Instance = new(() => m_Instance.GetValue(null));
        public static object Instance
        {
            get => _Instance.Value;
        }

        public static APIUser SelectedUser
        {
            get => m_APIUsers.Select(info => (APIUser)info.GetValue(_Instance.Value)).FirstOrDefault(user => user is not null);
            set => SelectUser(value);
        }

        public static VRCPlayerApi SelectedPlayer
        {
            get
            {
                APIUser selected = SelectedUser;
                if (selected == null)
                    return null;

                return VRCPlayerApi.AllPlayers.Find(
                    UnhollowerRuntimeLib.DelegateSupport.ConvertDelegate<Il2CppSystem.Predicate<VRCPlayerApi>>(
                        new Predicate<VRCPlayerApi>(player => player.displayName == selected.displayName)
                    )
                );
            }
        }

        // todo: bake this to a delegate
        public static void SelectUser(APIUser user) => m_SelectUser.Invoke(Instance, new object[1] { user });
    }
}
