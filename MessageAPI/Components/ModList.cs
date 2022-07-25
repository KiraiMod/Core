using System.Collections.Generic;
using System.Linq;

namespace KiraiMod.Core.MessageAPI.Components
{
    [ModuleAPI.Module]
    public static class ModList
    {
        // todo: an option to hide the tags
        //       tagapi needs an ability to hide all of a tag type

        [ModuleAPI.Configure<bool>("Visuals.ModList.Enabled", true)]
        public static bool Enabled;

        public static readonly List<string> Mods = new();
        private static readonly Dictionary<string, TagAPI.TagData> PlayerData = new();
        private static readonly TagAPI.Tag tag = new(player =>
            PlayerData.TryGetValue(player.VRCPlayerApi.displayName, out TagAPI.TagData data)
                ? data
                : new(),
            500
        );

        static ModList()
        {
            // potential exception if VRCPlayerApi or displayName is null:
            Events.Player.Left += player => PlayerData.Remove(player.VRCPlayerApi.displayName);
            Events.World.Unloaded += _ => PlayerData.Clear();

            Managers.RPCManager.listeners.Add((int)CoreIDs.AnnouncePresence, (sender, message) =>
            {
                if (!Enabled || sender.VRCPlayerApi.isLocal) return;

                new Message(
                    (int)CoreIDs.RequestModList, 
                    new MessageHeader[] { new Headers.TargetHeader(sender.VRCPlayerApi.displayName) },
                    System.Text.Encoding.UTF8.GetBytes(string.Join('\0', Mods))
                ).Send();
            });

            Managers.RPCManager.listeners.Add((int)CoreIDs.RequestModList, (sender, message) =>
            {
                if (!Enabled || sender.VRCPlayerApi.isLocal) return;

                Headers.TargetHeader header = message.GetHeader<Headers.TargetHeader>();
                if (header == null || !header.Targets.Contains(VRC.Core.APIUser.CurrentUser.displayName)) return;

                AddModsFromMessage(sender, message);

                if (Mods.Count == 0) return;

                new Message(
                    (int)CoreIDs.SendModList,
                    new MessageHeader[] { new Headers.TargetHeader(sender.VRCPlayerApi.displayName) },
                    System.Text.Encoding.UTF8.GetBytes(string.Join('\0', Mods))
                ).Send();
            });

            Managers.RPCManager.listeners.Add((int)CoreIDs.SendModList, (sender, message) =>
            {
                Headers.TargetHeader header = message.GetHeader<Headers.TargetHeader>();
                if (header == null || !header.Targets.Contains(VRC.Core.APIUser.CurrentUser.displayName)) return;

                AddModsFromMessage(sender, message);
            });
        }

        private static void AddModsFromMessage(Types.Player sender, Message message)
        {
            string[] mods = message.BodyString.Split(',');

            PlayerData[sender.VRCPlayerApi.displayName] = new()
            {
                Visible = true,
                Text = string.Join(" | ", message.BodyString.Split("\0").Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x))),
                BackgroundColor = UnityEngine.Color.black,
                TextColor = UnityEngine.Color.white
            };

            tag.CalculateAll();
        }
    }
}
