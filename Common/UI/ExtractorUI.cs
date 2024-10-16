using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using BiomeExtractorsMod.Common.Players;

namespace BiomeExtractorsMod.Common.UI
{
    internal class ExtractorUI : UIState
    {
        internal static float PanelWidth => UISlotArea.AreaWidth + 24f;
        UIPanel panel;
        UIText header;
        UIPanel button;
        UIText biomeText;
        UISlotArea slotArea;

        bool Dragging = false;
        Vector2 CursorDragOffset;
        readonly float panelSnap = 36f;
        private float DraggingAreaHeight => slotArea.Top.Pixels + 12;

        public override void OnInitialize()
        {
            panel = new();
            panel.Top.Set(0f, 0f);
            panel.Left.Set(0f, 0f);
            Append(panel);
            panel.OnLeftMouseDown += MovePanel;
            panel.OnLeftMouseUp += StopMovingPanel;

            header = new("", 1.2f);
            header.HAlign = 0.5f;
            header.Top.Set(10f, 0f);
            panel.Append(header);
            header.OnLeftMouseDown += MovePanel;
            header.OnLeftMouseUp += StopMovingPanel;

            biomeText = new("");
            biomeText.Left.Set(0f, 0f);
            biomeText.Top.Set(40f, 0f);
            biomeText.IsWrapped = true;
            biomeText.TextOriginX = 0f;
            panel.Append(biomeText);
            biomeText.OnLeftMouseDown += MovePanel;
            biomeText.OnLeftMouseUp += StopMovingPanel;

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

            slotArea = new();
            slotArea.HAlign = 0.5f;
            panel.Append(slotArea);

            panel.Width.Set(PanelWidth, 0f);
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
                int slot = slotArea.Hovered + slotArea.TopRow * UISlotArea.Columns;
                if (slot < slotArea.SlotData.Length)
                {
                    SlotData data = slotArea.SlotData[slot];
                    string name = data.Item.Name;
                    string chance = Language.GetTextValue($"{BiomeExtractorsMod.LocDiagnostics}.Chance") + $": {data.ChanceString}";
                    string daily = "";
                    UISystem uisys = ModContent.GetInstance<UISystem>();
                    if (uisys is not null)
                    {
                        double rolls = (86400 / uisys.tier.Rate) * (uisys.tier.Chance / 100.0);
                        daily = data.DailyString(rolls) + " " + Language.GetTextValue($"{BiomeExtractorsMod.LocDiagnostics}.Per_day");
                    }

                    SetTooltip($"{name}\n[c/FFFFFF:{chance}]\n[c/FFFFFF:{daily}]", data.Item.rare);
                }
            }
            if (Dragging)
            {
                Vector2 mouse = UserInterface.ActiveInstance.MousePosition;
                Vector2 newPos = mouse + CursorDragOffset;
                panel.Left.Set(newPos.X, 0f);
                panel.Top.Set(newPos.Y, 0f);
            } else {
                float draggingAreaRight = panel.Left.Pixels + panel.Width.Pixels;
                float draggingAreaBottom = panel.Top.Pixels + DraggingAreaHeight;
                float screenRightLimit = Main.screenWidth - panelSnap;
                float screenBottomLimit = Main.screenHeight - panelSnap;
                if (draggingAreaRight < panelSnap) panel.Left.Set(-panel.Width.Pixels + panelSnap, 0f);
                else if (panel.Left.Pixels > screenRightLimit) panel.Left.Set(screenRightLimit, 0f);
                if (draggingAreaBottom < panelSnap) panel.Top.Set(-DraggingAreaHeight + panelSnap, 0f);
                else if (panel.Top.Pixels > screenBottomLimit) panel.Top.Set(screenBottomLimit, 0f);
            }
        }

        private void MovePanel(UIMouseEvent evt, UIElement listeningElement)
        {
            Vector2 panelPos = new(panel.Left.Pixels, panel.Top.Pixels);
            Vector2 cursorPos = new(evt.MousePosition.X, evt.MousePosition.Y);
            CursorDragOffset = panelPos - cursorPos;
            if(CursorDragOffset.Y > -DraggingAreaHeight)
                Dragging = true;
        }

        private void StopMovingPanel(UIMouseEvent evt, UIElement listeningElement)
        {
            Dragging = false;
        }

        public override void OnActivate()
        {
            UISystem uisys = ModContent.GetInstance<UISystem>();
            if (uisys is not null)
            {
                header.SetText(uisys.GetWindowTitle());
                string status = uisys.GetExtractorStatus();
                biomeText.SetText(status);

                DynamicSpriteFont font = FontAssets.MouseText.Value;
                float spacing = font.LineSpacing;
                string visibleText = font.CreateWrappedText(status, panel.Width.Pixels-23f);
                float height = (visibleText.Split('\n').Length * spacing);

                slotArea.Top.Set(40f + height, 0f);
                slotArea.InitElements(uisys.GetDropList());

                Vector2 pos = ExtractorPlayer.LocalPlayer.ExtractorWindowPos;
                panel.Top.Set(pos.Y, 0f);
                panel.Left.Set(pos.X, 0f);
                panel.Width.Set(PanelWidth, 0f);
            }
            panel.Height.Set(slotArea.Top.Pixels + slotArea.Height.Pixels + 30f, 0f);
        }

        public override void OnDeactivate()
        {
            ExtractorPlayer player = ExtractorPlayer.LocalPlayer;
            player.ExtractorWindowPos = new(panel.Left.Pixels, panel.Top.Pixels);
            slotArea.SlotData = [];
            Dragging = false;
        }

        private static void SetTooltip(string text, int rarity)
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
