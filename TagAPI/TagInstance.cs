using UnityEngine;

namespace KiraiMod.Core.TagAPI
{
    public class TagInstance
    {
        public PlayerData player;

        public Transform parent;
        //public ImageThreeSlice image;
        public TMPro.TextMeshProUGUI text;

        public Tag tag;

        public TagInstance(PlayerData player, Tag tag, Transform transform)
        {
            this.player = player;
            this.tag = tag;

            parent = transform.parent;
            //image = parent.GetComponent<ImageThreeSlice>();
            text = transform.GetComponent<TMPro.TextMeshProUGUI>();
        }

        public void Calculate()
        {
            if (parent == null) return;

            TagData data = tag.evaluator(player.player);

            if (data.Visible != parent.gameObject.activeSelf)
            {
                player.RecalculatePositions();

                if (!(parent.gameObject.active = data.Visible))
                    return;
            }

            if (data.Text != null) text.text = data.Text;
            if (data.TextColor != null) text.color = data.TextColor;
            //if (data.BackgroundColor != null) image.color = data.BackgroundColor;
        }
    }
}
