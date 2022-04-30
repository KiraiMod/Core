using KiraiMod.Core.ModuleAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace KiraiMod.Core.Managers
{
    public static class ModuleManager
    {
        static ModuleManager() => Register(Assembly.GetExecutingAssembly());

        public static void Register() => Register(Assembly.GetCallingAssembly());
        public static void Register(Assembly assembly)
        {
            IEnumerable<ModuleAttribute> modules = assembly.GetExportedTypes()
                .Select(t =>
                {
                    var attribute = t.GetCustomAttribute<ModuleAttribute>();
                    if (attribute != null)
                        attribute.__declarer = t;
                    return attribute;
                })
                .Where(x => x is not null);

            modules.ForEach(x => {
                Plugin.Logger.LogDebug("Initializing " + x.__declarer.FullName);
                try { x.__declarer.Initialize(); }
                catch (Exception ex) { Plugin.Logger.LogError("Exception occurred whilst loading " + x.__declarer.FullName + ": " + ex); }
            });
        }
    }
}
