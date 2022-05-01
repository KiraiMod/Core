using System;
using System.Linq;
using System.Reflection;
using TypeScanner.Types;
using UnityEngine;

namespace KiraiMod.Core.Types
{
    public static class PortalInternal
    {
        public static Type Type = ClassDef.Create("PortalInternal")
            .FromAssembly(Utils.Misc.AssemblyCSharp)
            .DerivesFrom<MonoBehaviour>()
            .WithMethods(
                MethodDef.Create()
                .WithName("ConfigurePortal")
                .WithParameters(
                    typeof(string),
                    typeof(string),
                    typeof(int),
                    null
                )
            )
            .Setup()
            .Resolved;

        public static MethodInfo m_CreatePortal = Type.GetMethods().FirstOrDefault(x =>
        {
            var parms = x.GetParameters();
            return parms.Length == 5
                && parms[0].ParameterType == typeof(VRC.Core.ApiWorld)
                && parms[1].ParameterType == typeof(VRC.Core.ApiWorldInstance)
                && parms[2].ParameterType == typeof(Vector3)
                && parms[3].ParameterType == typeof(Vector3);
        });

        static PortalInternal()
        {
            Type.LogAs(nameof(PortalInternal));
            m_CreatePortal.LogAs(".CreatePortal");
        }
    }
}
