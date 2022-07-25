using System;
using System.Collections.Generic;
using UnhollowerBaseLib.Attributes;
using UnhollowerRuntimeLib;
using UnityEngine;

namespace KiraiMod.Core.TagAPI
{
    public class PlayerData : MonoBehaviour
    {
        static PlayerData() => ClassInjector.RegisterTypeInIl2Cpp<PlayerData>();

        public Types.Player player;

        public List<TagInstance> tags = new();
        public Transform contents;
        public Transform stats;

        public PlayerData() : base(ClassInjector.DerivedConstructorPointer<PlayerData>()) => ClassInjector.DerivedConstructorBody(this);
        public PlayerData(IntPtr ptr) : base(ptr) { }

        ~PlayerData()
        {
            foreach (TagInstance inst in tags)
                inst.tag.instances.Remove(inst);
        }

        public void Setup()
        {
            contents = player.Inner.transform.Find("Player Nameplate/Canvas/Nameplate/Contents");
            stats = contents.Find("Quick Stats");
        }

        [HideFromIl2Cpp]
        public void Create(Tag tag)
        {
            Transform rank = Instantiate(stats, stats.parent, false);
            rank.name = $"AstralTag";
            rank.gameObject.active = true;
            Transform transform = null;

            for (int i = rank.childCount; i > 0; i--)
            {
                Transform child = rank.GetChild(i - 1);

                if (child.name == "Trust Text")
                {
                    transform = child;
                    continue;
                }

                Object.Destroy(child.gameObject);
            }

            TagInstance inst = new(this, tag, transform);

            tags.Add(inst);
            tag.instances.Add(inst);

            inst.Calculate();
            RecalculatePositions();
        }

        [HideFromIl2Cpp]
        public void RecalculatePositions()
        {
            int i = 1;
            foreach (TagInstance tag in tags)
                if (tag.parent.gameObject.activeSelf)
                    tag.parent.localPosition = new Vector3(0, 30 * ++i, 0);
        }
    }
}
