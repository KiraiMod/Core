using KiraiMod.Core.ModuleAPI;
using KiraiMod.Core.ModuleAPI.Subtypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace KiraiMod.Core.Managers
{
    public static class ModuleManager
    {
        public static Dictionary<string, Section> Sections = new();
        public static event Action<string, Section> SectionCreated;

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

            var members = module.Type.GetMembers()
                .SelectMany(x =>
                {
                    var members = x.GetCustomAttributes<MemberAttribute>();
                    foreach (MemberAttribute member in members)
                        member.Setup(module.Type, x);
                    return members;
                })
                .GroupBy(x => x.Section);

            members.ForEach(x =>
            {
                if (!Sections.TryGetValue(x.Key, out Section section))
                {
                    section = new();
                    Sections[x.Key] = section;
                    SectionCreated?.StableInvoke(x.Key, section);
                }

                section.Members.AddRange(x);
                section.InvokeEvent(x.ToArray());
            });
        }

        public class Section
        {
            public List<MemberAttribute> Members = new();
            public event Action<MemberAttribute[]> MembersAdded;

            internal void InvokeEvent(MemberAttribute[] members) => MembersAdded?.StableInvoke(members);
        }
    }
}
