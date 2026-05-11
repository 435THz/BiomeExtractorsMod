using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;
using BiomeExtractorsMod.Common.Players;
using Terraria.ID;
using Terraria.Localization;
using Terraria.GameContent;

namespace BiomeExtractorsMod.Common.UI
{
    internal class AnalyzerUI : UIState
    {
        private static UISystem uisys => ModContent.GetInstance<UISystem>();
        internal static float PanelWidth => UISlotArea.AreaWidth + 24f;
        UIPanel panel;
        UIText header;
        UIPanel analyzeButton;
        UIPanel closeButton;
        UIScrollableText scanResult;
        UIScanSlot scanSlot;

        bool Dragging = false;
        Vector2 CursorDragOffset;
        readonly float panelSnap = 36f;
        private float DraggingAreaHeight => scanSlot.Top.Pixels + 12;

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

            scanSlot = new(0);
            scanSlot.Top.Set(40f, 0f);
            scanSlot.HAlign = 0.5f;
            panel.Append(scanSlot);
            scanSlot.OnLeftMouseDown += OnSlotClick;

            scanResult = new();
            scanResult.Top.Set(80f, 0f);
            scanResult.Left.Set(0f, 0f);
            panel.Append(scanResult);

            analyzeButton = new();
            analyzeButton.Width.Set(36f, 0f);
            analyzeButton.Height.Set(36f, 0f);
            analyzeButton.Top.Set(2f, 0f);
            analyzeButton.Left.Set(2f, 0f);
            analyzeButton.OnLeftClick += OnAnalyzeClick;
            panel.Append(analyzeButton);

            UIImage magGlass = new(TextureAssets.Cursors[CursorOverrideID.Magnifiers]);
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

            panel.Width.Set(PanelWidth, 0f);
            scanResult.Width.Set(panel.Width.Pixels, 0f);
        }

        private void OnSlotClick(UIMouseEvent evt, UIElement listeningElement)
        {
            
			Player player = Main.LocalPlayer;
			if (player.itemAnimation == 0 && player.itemTime == 0)
            {
				scanSlot.itemId = Main.mouseItem.type;

                string text = uisys.GeneratePoolsText(scanSlot.itemId);
                
                scanResult.SetText(text);
            }
        }

        private void OnAnalyzeClick(UIMouseEvent evt, UIElement listeningElement)
        {
            uisys.SwitchToScannerInterface();
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
            if (scanSlot.Hovered && scanSlot.itemId != ItemID.None)
            {
                Item item = new(scanSlot.itemId);
                string tooltip = item.Name;
                for (int i = 0; i < item.ToolTip.Lines; i++)
                    tooltip += "\n" + item.ToolTip.GetLine(i);
                SetTooltip(tooltip, item.rare);

            }
            if (Dragging)
            {
                Vector2 mouse = UserInterface.ActiveInstance.MousePosition;
                Vector2 newPos = mouse + CursorDragOffset;
                panel.Left.Set(newPos.X, 0f);
                panel.Top.Set(newPos.Y, 0f);
            }
            else
            {
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
            if (CursorDragOffset.Y > -DraggingAreaHeight)
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
                header.SetText(uisys.GetWindowTitle());
                scanResult.SetText(Language.GetTextValue($"{BiomeExtractorsMod.LocAnalyzer}.Empty"));

                if (uisys.UIHolder.CurrentState is null)
                {
                    Vector2 pos = ExtractorPlayer.LocalPlayer.ExtractorWindowPos;
                    panel.Top.Set(pos.Y, 0f);
                    panel.Left.Set(pos.X, 0f);
                }
                else if(uisys.switching)
                {
                    Vector2 pos = uisys.ExtractorInterface.GetPos();
                    panel.Top.Set(pos.Y, 0f);
                    panel.Left.Set(pos.X, 0f);
                }
                panel.Width.Set(PanelWidth, 0f);
            }
            panel.Height.Set(300f, 0f);
            uisys.switching = false;
        }

        public override void OnDeactivate()
        {
            ExtractorPlayer player = ExtractorPlayer.LocalPlayer;
            player.ExtractorWindowPos = new(panel.Left.Pixels, panel.Top.Pixels);
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
    }
}
