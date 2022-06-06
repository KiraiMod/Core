using System;
using UnhollowerBaseLib.Attributes;
using UnhollowerRuntimeLib;
using UnityEngine;

namespace KiraiMod.Core.Utils
{
    public class ActivationListener : MonoBehaviour
    {
        static ActivationListener() => ClassInjector.RegisterTypeInIl2Cpp<ActivationListener>();

        [HideFromIl2Cpp] 
        public event Action Enabled;

        [HideFromIl2Cpp] 
        public event Action Disabled;

        public ActivationListener(IntPtr ptr) : base(ptr) { }

        public void OnEnable() => Enabled?.StableInvoke();
        public void OnDisable() => Disabled?.StableInvoke();
    }
}
