global using Object = UnityEngine.Object;

using BepInEx;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiraiMod.Core
{
    [BepInPlugin("me.kiraihooks.KiraiMod.Core", "KM.Core", "latest")]
    public class Plugin : BasePlugin
    {
        internal static ManualLogSource Logger;

        public override void Load()
        {
            Logger = Log;

            typeof(Events).Initialize();
        }
    }
}
