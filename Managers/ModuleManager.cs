using BepInEx.Configuration;
using KiraiMod.Core.ModuleAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace KiraiMod.Core.Managers
{
    public static class ModuleManager
    {
        internal static readonly ConfigFile Config = new("BepInEx/config/KMCoreModuleAPI.cfg", false);

        static ModuleManager() => Register(Assembly.GetExecutingAssembly());

        public static void Register() => Register(Assembly.GetCallingAssembly());
        public static void Register(Assembly assembly)
        {
            IEnumerable<ModuleAttribute> modules = assembly.GetExportedTypes()
                .Select(t =>
                {
                    var attribute = t.GetCustomAttribute<ModuleAttribute>();
                    if (attribute != null)
                        attribute.Type = t;
                    return attribute;
                })
                .Where(x => x is not null);

            modules.ForEach(x =>
            {
                Plugin.Logger.LogDebug("Initializing " + x.Type.FullName);
                try { SetupModule(x); }
                catch (Exception ex) { Plugin.Logger.LogError("Exception occurred whilst loading " + x.Type.FullName + ": " + ex); }
            });
        }

        public static void SetupModule(ModuleAttribute module)
        {
            module.Type.Initialize();

            module.Type.GetMembers()
                .ForEach(x =>
                {
                    var members = x.GetCustomAttributes<MemberAttribute>();
                    foreach (MemberAttribute member in members)
                        member.SetupInternal(module.Type, x);
                });
        }
    }
}
