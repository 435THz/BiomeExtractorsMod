using Microsoft.Xna.Framework;
using ReLogic.Graphics;
using System;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace BiomeExtractorsMod.Common.UI
{
    internal class UIScrollableText : UIElement
    {

        internal static float AreaWidth = UISlotArea.AreaWidth;
        internal static float AreaHeight = UISlotArea.AreaHeight;
        internal static int VisibleLines = (int)AreaHeight/FontAssets.MouseText.Value.LineSpacing;

        internal UIText[] LineContainers = [];
        internal string[] Lines;
        private readonly UIScrollbar scrollbar;

        internal int Hovered = -1;
        private int  _topRow = 0;
        internal int TopRow
        {
            get => _topRow;
            set
            {
                _topRow = Math.Clamp(value, 0, Math.Max(0, MaxLines-VisibleLines));
            }
        }
        private float _previousViewPosition = 0f;
        internal int MaxLines => Lines.Length;

        internal UIScrollableText() : base()
        {
            Lines = [];
            LineContainers = new UIText[VisibleLines];

            for(int i=0; i<VisibleLines; i++)
            {
                LineContainers[i] = new UIText("");
                LineContainers[i].TextOriginX = 0f;
            }

            scrollbar = new();
            scrollbar.Top.Set(0, 0);
            scrollbar.Left.Set(AreaWidth-20f, 0f);
            scrollbar.Height.Set(AreaHeight, 0f);
            scrollbar.SetView(VisibleLines, MaxLines);
            Append(scrollbar);

            Width.Set(AreaWidth, 0f);
            Height.Set(AreaHeight, 1f);
        }

        public void SetText(string text)
        {
            DynamicSpriteFont font = FontAssets.MouseText.Value;
            string visibleText = font.CreateWrappedText(text, AreaWidth-23f);
            Lines = visibleText.Split('\n');
            scrollbar.ViewPosition = 0;
            
            UpdateLines();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (scrollbar.ViewPosition != _previousViewPosition)
            {
                _previousViewPosition = scrollbar.ViewPosition;
                TopRow = (int)Math.Round(scrollbar.ViewPosition);
                UpdateLines();
            }
        }

        public override void ScrollWheel(UIScrollWheelEvent evt)
        {
            base.ScrollWheel(evt);

            if (IsMouseHovering)
            {
                if (evt.ScrollWheelValue > 0)
                {
                    scrollbar.ViewPosition--;
                }
                else if (evt.ScrollWheelValue < 0)
                {
                    scrollbar.ViewPosition++;
                }
            }
        }

        internal void UpdateLines()
        {
            for (int x = 0; x < VisibleLines; x++)
            {
                int slot = VisibleLines + x;
                string text = "";
                if (slot < MaxLines)
                {
                    text = Lines[slot];
                }
                LineContainers[x].SetText(text);
            }
        }
    }
}
