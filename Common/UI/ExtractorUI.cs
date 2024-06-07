using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;

namespace BiomeExtractorsMod.Common.UI
{
    internal class ExtractorUI : UIState
    {
        UIPanel panel;
        UIText header;
        UIPanel button;
        UIText biomeText;
        public override void OnInitialize()
        {
            panel = new UIPanel();
            panel.Width.Set(500, 0);
            panel.Height.Set(400, 0);
            panel.Top.Set(0, 0.025f);
            panel.Left.Set(-panel.Width.Pixels / 2, 0.5f);
            Append(panel);

            header = new UIText("", 1.2f);
            header.HAlign = 0.5f;
            header.Top.Set(10, 0);
            panel.Append(header);

            button = new UIPanel();
            button.Width.Set(36, 0);
            button.Height.Set(36, 0);
            button.HAlign = 1f;
            button.Top.Set(2, 0);
            button.Left.Set(-2, 0);
            button.OnLeftClick += OnButtonClick;
            panel.Append(button);

            UIText text = new("X");
            text.HAlign = text.VAlign = 0.5f;
            button.Append(text);

            biomeText = new UIText("");
            biomeText.Left.Set(0, 0);
            biomeText.Top.Set(40, 0);
            biomeText.IsWrapped = true;
            biomeText.Width.Set(500, 0);
            biomeText.TextOriginX = 0;
            panel.Append(biomeText);
        }

        private void OnButtonClick(UIMouseEvent evt, UIElement listeningElement)
        {
            ModContent.GetInstance<UISystem>().CloseInterface();
        }
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            if (panel.ContainsPoint(Main.MouseScreen))
            {
                Main.LocalPlayer.mouseInterface = true;
            }
            if (panel.IsMouseHovering)
            {
                PlayerInput.LockVanillaMouseScroll("BiomeExtractorsMod/LootTable");
            }
        }

        public override void OnActivate()
        {
            UISystem uisys = ModContent.GetInstance<UISystem>();
            if (uisys is not null)
            {
                header.SetText(uisys.GetExtractorName());
                biomeText.SetText(uisys.GetExtractorStatus());
            }
        }

        public override void OnDeactivate()
        {}
    }
}
