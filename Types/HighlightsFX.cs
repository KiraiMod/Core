using System;
using System.Linq;
using System.Reflection;
using TypeScanner.Types;
using UnityEngine;

namespace KiraiMod.Core.Types
{
    public static class HighlightsFX
    {
        public static Type Type = ClassDef.Create("HighlightsFX")
            .FromAssembly(Utils.Misc.AssemblyCSharp)
            .ConstructorCount(2)
            .WithMethods(
                MethodDef.Create().WithName("Awake"),
                MethodDef.Create().WithName("OnDestroy"),
                MethodDef.Create().WithParameters(typeof(Renderer), typeof(bool))
            )
            .Setup()
            .Resolved;

        public static PropertyInfo m_Instance = Type.Singleton();
        public static MethodInfo m_HighlightRenderer = Type.GetMethods().FirstOrDefault(x => !x.IsStatic && x.GetParameters().Length == 2);

        public static object _Instance;
        public static object Instance
        {
            get {
                if (_Instance == null)
                    _Instance = m_Instance.GetValue(null);
                return _Instance;
            }
        }

        static HighlightsFX() => Type.LogAs("HighlightsFX");

        // todo: bake to a delegate
        public static void Highlight(Renderer renderer, bool enabled) => m_HighlightRenderer.Invoke(Instance, new object[] { renderer, enabled });
    }
}
