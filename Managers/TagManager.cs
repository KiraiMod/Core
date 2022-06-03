using KiraiMod.Core.TagAPI;
using System.Collections.Generic;

namespace KiraiMod.Core.Managers
{
    public static class TagManager
    {
        public static List<PlayerData> PlayerTags = new();

        static TagManager() => Events.Player.Joined += CreatePlayerTag;

        public static void CreatePlayerTag(Types.Player player)
        {
            PlayerData data = player.Inner.gameObject.AddComponent<PlayerData>();
            data.player = player;
            data.Setup();

            Tag.TagRegistered += data.Create;

            foreach (Tag tag in Tag.tags)
                data.Create(tag);
        }
    }
}
