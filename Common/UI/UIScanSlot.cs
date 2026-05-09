using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace BiomeExtractorsMod.Common.UI
{

    internal class UIScanSlot : UIPanel
    {
        internal static int size = 45;
        internal UIItemIcon icon;
        internal int itemId;
        internal bool Hovered = false;
        internal UIScanSlot(int itemId)
        {
            this.itemId = itemId;
            Width.Set(size, 0);
            Height.Set(size, 0);
            icon = new(new(0), false);
            icon.HAlign = icon.VAlign = 0.5f;
            Append(icon);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        internal void SetItem(int itemId)
        {
            RemoveChild(icon);
            this.itemId = itemId;
            icon = new(new(this.itemId), false);
            Append(icon);
            
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
            Hovered = true;
        }

        public override void MouseOut(UIMouseEvent evt)
        {
            base.MouseOut(evt);
            Hovered = false;
        }
    }
}
