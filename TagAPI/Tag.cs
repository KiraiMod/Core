using System;
using System.Collections.Generic;
using System.Linq;

namespace KiraiMod.Core.TagAPI
{
    public class Tag
    {
        public static List<Tag> tags = new();
        public static event Action<Tag> TagRegistered;

        static Tag() => typeof(Managers.TagManager).Initialize();

        public readonly int priority;
        public readonly List<TagInstance> instances = new();
        internal readonly Func<Types.Player, TagData> evaluator;

        public Tag(Func<Types.Player, TagData> evaluator, int priority = 0)
        {
            this.evaluator = evaluator;
            this.priority = priority;

            tags.Add(this);
            tags = tags.OrderByDescending(tag => tag.priority).ToList();
            TagRegistered?.StableInvoke(this);
        }

        public void CalculateAll() => instances.ForEach(x => x?.Calculate());
    }
}
