using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using VRC.Core;
using VRC.SDKBase;

namespace KiraiMod.Core.Types
{
    public class Player
    {
        public static readonly Type Type = Events.Hooks.AssemblyCSharp.GetExportedTypes()
            .Where(x => x.BaseType == typeof(MonoBehaviour))
            .Where(x => x.GetConstructors().Length == 2)
            .Where(x => x.GetMethod("OnNetworkReady") != null)
            .Where(f => {
                var props = f.GetProperties();
                return props.Any(f => f.PropertyType == typeof(VRCPlayerApi))
                    && props.Any(f => f.PropertyType == typeof(APIUser));
            })
            .FirstOrDefault();

        private static readonly PropertyInfo m_APIUser = Type?.GetProperties().FirstOrDefault(f => f.PropertyType == typeof(APIUser));
        private static readonly PropertyInfo m_VRCPlayerApi = Type?.GetProperties().FirstOrDefault(f => f.PropertyType == typeof(VRCPlayerApi));

        static Player() => Type.LogAs(nameof(Player));

        public readonly MonoBehaviour Inner;
        public readonly Lazy<APIUser> _APIUser;
        public readonly Lazy<VRCPlayerApi> _VRCPlayerApi;

        public APIUser APIUser { get => _APIUser.Value; }
        public VRCPlayerApi VRCPlayerApi { get => _VRCPlayerApi.Value; }

        public Player(MonoBehaviour inner)
        {
            Inner = inner;
            _APIUser = new(() => m_APIUser?.GetValue(inner) as APIUser);
            _VRCPlayerApi = new(() => m_VRCPlayerApi?.GetValue(inner) as VRCPlayerApi);
        }

        // dude, these still need testing
        public override int GetHashCode() => this?.Inner.GetHashCode() ?? 0;
        public override bool Equals(object obj) => this?.Inner.Equals(obj) ?? obj is null;
        public static bool operator ==(Player self, Player other) => ReferenceEquals(self?.Inner, other?.Inner);
        public static bool operator !=(Player self, Player other) => !(self == other);
    }
}
