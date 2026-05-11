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
using Terraria.ID;
using Terraria.Audio;

namespace BiomeExtractorsMod.Common.UI
{
    internal class ExtractorUI : UIState
    {
        private static UISystem uisys => ModContent.GetInstance<UISystem>();
        internal static float PanelWidth => UISlotArea.AreaWidth + 24f;
        UIPanel panel;
        UIText header;
        UIPanel analyzeButton;
        UIPanel closeButton;
        UIText biomeText;
        UISlotArea slotArea;

        bool Dragging = false;
        Vector2 CursorDragOffset;
        readonly float panelSnap = 36f;
        private float DraggingAreaHeight => slotArea.Top.Pixels + 12;

        internal Vector2 GetPos()
        {
            return new Vector2(panel.Left.Pixels, panel.Top.Pixels);
        }
        public override void OnInitialize()
        {
            panel = new();
            panel.Top.Set(0f, 0f);
            panel.Left.Set(0f, 0f);
            Append(panel);
            panel.OnLeftMouseDown += MovePanel;
            panel.OnLeftMouseUp += StopMovingPanel;

            header = new("", 1.2f);
            header.Top.Set(10f, 0f);
            header.HAlign = 0.5f;
            panel.Append(header);
            header.OnLeftMouseDown += MovePanel;
            header.OnLeftMouseUp += StopMovingPanel;

            biomeText = new("");
            biomeText.Top.Set(40f, 0f);
            biomeText.Left.Set(0f, 0f);
            biomeText.IsWrapped = true;
            biomeText.TextOriginX = 0f;
            panel.Append(biomeText);
            biomeText.OnLeftMouseDown += MovePanel;
            biomeText.OnLeftMouseUp += StopMovingPanel;

            analyzeButton = new();
            analyzeButton.Width.Set(36f, 0f);
            analyzeButton.Height.Set(36f, 0f);
            analyzeButton.Top.Set(2f, 0f);
            analyzeButton.Left.Set(2f, 0f);
            analyzeButton.OnLeftClick += OnAnalyzeClick;
            panel.Append(analyzeButton);

            UIImage magGlass = new(TextureAssets.Cursors[CursorOverrideID.Magnifiers]);
            magGlass.Top.Set(-5f, 0f);
            magGlass.Left.Set(-5f, 0f);
            magGlass.HAlign = 0.5f;
            magGlass.VAlign = 0.5f;
            analyzeButton.Append(magGlass);
            
            closeButton = new();
            closeButton.Width.Set(36f, 0f);
            closeButton.Height.Set(36f, 0f);
            closeButton.HAlign = 1f;
            closeButton.Top.Set(2f, 0f);
            closeButton.Left.Set(-2f, 0f);
            closeButton.OnLeftClick += OnExitClick;
            panel.Append(closeButton);

            UIText text = new("X");
            text.HAlign = text.VAlign = 0.5f;
            closeButton.Append(text);

            slotArea = new();
            slotArea.HAlign = 0.5f;
            slotArea.OnLeftClick += ToggleClickedItem;
            panel.Append(slotArea);

            panel.Width.Set(PanelWidth, 0f);
            biomeText.Width.Set(panel.Width.Pixels, 0f);
        }
        private void OnAnalyzeClick(UIMouseEvent evt, UIElement listeningElement)
        {
            uisys.SwitchToAnalyzerInterface();
        }
        private void OnExitClick(UIMouseEvent evt, UIElement listeningElement)
        {
            uisys.CloseInterface();
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
                    if (uisys is not null)
                    {
                        double rolls = (86400 / uisys.tier.Rate) * (uisys.tier.Chance / 100.0) * uisys.tier.Amount;
                        daily = $"{data.DailyString(rolls)} {Language.GetTextValue($"{BiomeExtractorsMod.LocDiagnostics}.Per_day")}";
                    }

                    string tooltip = $"{name}\n[c/FFFFFF:{chance}]\n[c/FFFFFF:{daily}]";
                    if (!data.IsActive)
                        tooltip += $"\n[c/828282:{Language.GetTextValue($"{BiomeExtractorsMod.LocDiagnostics}.InactiveSlot")}]";
                    SetTooltip(tooltip, data.Item.rare);
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
            if (uisys is not null)
            {
                if(uisys.isAnalyzer) {
                    if(!panel.HasChild(analyzeButton))
                        panel.Append(analyzeButton);
                }
                else
                {
                    if(panel.HasChild(analyzeButton))
                        panel.RemoveChild(analyzeButton);
                }
                header.SetText(uisys.GetWindowTitle());
                string status = uisys.GetExtractorStatus();
                biomeText.SetText(status);

                DynamicSpriteFont font = FontAssets.MouseText.Value;
                float spacing = font.LineSpacing;
                string visibleText = font.CreateWrappedText(status, panel.Width.Pixels-23f);
                float height = visibleText.Split('\n').Length * spacing;

                slotArea.Top.Set(40f + height, 0f);
                slotArea.InitElements();

                if (uisys.UIHolder.CurrentState is null)
                {
                    Vector2 pos = ExtractorPlayer.LocalPlayer.ExtractorWindowPos;
                    panel.Top.Set(pos.Y, 0f);
                    panel.Left.Set(pos.X, 0f);
                }
                else if(uisys.switching)
                {
                    Vector2 pos = uisys.AnalyzerInterface.GetPos();
                    panel.Top.Set(pos.Y, 0f);
                    panel.Left.Set(pos.X, 0f);
                }
                panel.Width.Set(PanelWidth, 0f);
                panel.Height.Set(slotArea.Top.Pixels + slotArea.Height.Pixels + 30f, 0f);
                uisys.switching = false;
            }
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
            fakeItem.SetDefaults(ItemID.None, noMatCheck: true);
            fakeItem.SetNameOverride(text);
            fakeItem.type = ItemID.IronPickaxe;
            fakeItem.scale = 0f;
            fakeItem.rare = rarity;
            fakeItem.value = -1;
            Main.HoverItem = fakeItem;
            Main.instance.MouseText("");
            Main.mouseText = true;
        }

        private void ToggleClickedItem(UIMouseEvent evt, UIElement listeningElement)
        {
            if (slotArea.Hovered >= 0 && uisys.Extractor is not null)
            {
                int slot = slotArea.Hovered + slotArea.TopRow * UISlotArea.Columns;
                if (slot < slotArea.SlotData.Length)
                {
                    Item item = slotArea.SlotData[slot].Item;
                    if (uisys.Extractor.FilterContains(item))
                    {
                        uisys.Extractor.RemoveFilter(slotArea.SlotData[slot].Item);
                        slotArea.SlotData[slot].IsActive = true;
                    }
                    else
                    {
                        uisys.Extractor.AddFilter(slotArea.SlotData[slot].Item);
                        slotArea.SlotData[slot].IsActive = false;
                    }
                    SoundEngine.PlaySound(SoundID.MenuTick);
                    slotArea.UpdateSlots();
                }
            }
        }
    }
}
