using HarmonyLib;
using System;
using System.Reflection;

namespace KiraiMod.Core.Utils
{
    public class ToggleHook
    {
        private static readonly Harmony harmony = new(Plugin.GUID + ".ToggleHook");
        
        private readonly MethodInfo original;
        private readonly MethodInfo prefix;
        private readonly MethodInfo postfix;
        private readonly HarmonyMethod prefixHM;
        private readonly HarmonyMethod postfixHM;
        private bool state;

        public ToggleHook(MethodInfo original, MethodInfo prefix = null, MethodInfo postfix = null)
        {
            if (prefix == null && postfix == null)
                throw new ArgumentNullException("Both prefix and postfix were null");

            this.original = original;
            this.prefix = prefix;
            this.postfix = postfix;
            prefixHM = prefix.ToHM();
            postfixHM = postfix.ToHM();
        }

        public ToggleHook Enable()
        {
            if (!state)
            {
                state = true;
                harmony.Patch(original, prefixHM, postfixHM);
            }

            return this;
        }

        public ToggleHook Disable()
        {
            if (state)
            {
                state = false;

                if (prefix != null) harmony.Unpatch(original, prefix);
                if (postfix != null) harmony.Unpatch(original, postfix);
            }

            return this;
        }

        public ToggleHook Toggle(bool? state = null)
        {
            state ??= !this.state;

            if ((bool)state) Enable();
            else Disable();
            return this;
        }
    }
}
