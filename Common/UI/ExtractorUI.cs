using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.Localization;
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
        UISlotArea slotArea;

        public override void OnInitialize()
        {
            panel = new();
            panel.Top.Set(0f, 0.025f);
            panel.HAlign = 0.5f;
            Append(panel);

            header = new("", 1.2f);
            header.HAlign = 0.5f;
            header.Top.Set(10f, 0f);
            panel.Append(header);

            button = new();
            button.Width.Set(36f, 0f);
            button.Height.Set(36f, 0f);
            button.HAlign = 1f;
            button.Top.Set(2f, 0f);
            button.Left.Set(-2f, 0f);
            button.OnLeftClick += OnButtonClick;
            panel.Append(button);

            UIText text = new("X");
            text.HAlign = text.VAlign = 0.5f;
            button.Append(text);

            biomeText = new("");
            biomeText.Left.Set(0f, 0f);
            biomeText.Top.Set(40f, 0f);
            biomeText.IsWrapped = true;
            biomeText.TextOriginX = 0f;
            panel.Append(biomeText);

            slotArea = new();
            slotArea.HAlign = 0.5f;
            panel.Append(slotArea);

            panel.Width.Set(slotArea.Width.Pixels+23f, 0f);
            biomeText.Width.Set(panel.Width.Pixels, 0f);
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
            if (slotArea.Hovered >= 0)
            {
                int slot = slotArea.Hovered + slotArea.TopRow * slotArea.Columns;
                if (slot < slotArea.SlotData.Length)
                {
                    SlotData data = slotArea.SlotData[slot];
                    string name = data.Item.Name;
                    string chance = Language.GetTextValue($"{BiomeExtractorsMod.LocDiagnostics}.Chance") + $": {data.ChanceString}";
                    string daily = "";
                    UISystem uisys = ModContent.GetInstance<UISystem>();
                    if (uisys is not null)
                    {
                        double rolls = (86400 / uisys.Extractor.ExtractionRate) * (uisys.Extractor.ExtractionChance / 100.0);
                        daily = data.DailyString(rolls) + " " + Language.GetTextValue($"{BiomeExtractorsMod.LocDiagnostics}.Per_day");
                    }

                    SetTooltip($"{name}\n[c/FFFFFF:{chance}]\n[c/FFFFFF:{daily}]", data.Item.rare);
                }
            }
        }

        public override void OnActivate()
        {
            UISystem uisys = ModContent.GetInstance<UISystem>();
            if (uisys is not null)
            {
                header.SetText(uisys.GetExtractorName());
                biomeText.SetText(uisys.GetExtractorStatus());
                slotArea.Top.Set(100f, 0f);
                slotArea.InitElements(uisys.Extractor.GetDropList());
            }
            panel.Height.Set(slotArea.Top.Pixels + slotArea.Height.Pixels + 30f, 0f);
        }

        public override void OnDeactivate()
        {
            slotArea.SlotData = [];
        }

        public static void SetTooltip(string text, int rarity)
        {
            Item fakeItem = new();
            fakeItem.SetDefaults(0, noMatCheck: true);
            fakeItem.SetNameOverride(text);
            fakeItem.type = 1;
            fakeItem.scale = 0f;
            fakeItem.rare = rarity;
            fakeItem.value = -1;
            Main.HoverItem = fakeItem;
            Main.instance.MouseText("");
            Main.mouseText = true;
        }
    }
}
