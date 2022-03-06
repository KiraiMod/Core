global using Object = UnityEngine.Object;

using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using BepInEx.Logging;

namespace KiraiMod.Core
{
    [BepInPlugin(GUID, "KM.Core", "0.0.0")]
    [BepInDependency("me.kiraihooks.KiraiMod.Loader", BepInDependency.DependencyFlags.SoftDependency)]
    public class Plugin : BasePlugin
    {
        public const string GUID = "me.kiraihooks.KiraiMod.Core";

        internal static ManualLogSource Logger;
        internal static ConfigFile Configuration;

        public override void Load()
        {
            Logger = Log;
            Configuration = Config;

            typeof(Utils.Misc).Initialize();
            typeof(Managers.KeybindManager).Initialize();
            typeof(Components.ScreenLogger).Initialize();
        }
    }
}
