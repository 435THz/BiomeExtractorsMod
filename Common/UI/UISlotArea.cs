using BiomeExtractorsMod.Common.Collections;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using static BiomeExtractorsMod.Common.Database.BiomeExtractionSystem;

namespace BiomeExtractorsMod.Common.UI
{
    internal class UISlotArea : UIElement
    {
        private static readonly int Padding = 4;

        internal static int Columns = 8;
        internal static int Rows = 4;
        internal static float AreaWidth = (UIResultSlot.size + Padding) * Columns + 20f;
        internal static float AreaHeight = (UIResultSlot.size + Padding) * Rows;

        private UIResultSlot[,] Slots { get; set; } = new UIResultSlot[1, 1] {
            { new UIResultSlot(0) }
        };
        internal SlotData[] SlotData { get; set; } = [];
        private readonly UIScrollbar scrollbar;

        internal int Hovered { get; set; } = -1;
        private int  _topRow = 0;
        internal int TopRow
        {
            get => _topRow;
            set
            {
                _topRow = Math.Clamp(value, 0, Math.Max(0, MaxRows-Rows));
            }
        }
        private float _previousViewPosition = 0f;
        internal int MaxRows => (int)Math.Ceiling(SlotData.Length / (double)Columns);

        internal UISlotArea() : base()
        {
            Slots = new UIResultSlot[Rows, Columns];
            for (int y = 0; y < Rows; y++)
            {
                for (int x = 0; x < Columns; x++)
                {
                    int slot = y * Columns + x;
                    UIResultSlot itemSlot = new(slot);
                    Slots[y, x] = itemSlot;
                    itemSlot.Top.Set((UIResultSlot.size + Padding) * y, 0f);
                    itemSlot.Left.Set((UIResultSlot.size + Padding) * x, 0f);
                    Append(itemSlot);
                }
            }
            scrollbar = new();
            scrollbar.Top.Set(0, 0);
            scrollbar.Left.Set((UIResultSlot.size + Padding) * Columns, 0f);
            scrollbar.Height.Set((UIResultSlot.size + Padding) * Rows, 0f);
            scrollbar.SetView(Rows, MaxRows);
            Append(scrollbar);

            Width.Set(AreaWidth, 0f);
            Height.Set(AreaHeight, 1f);
        }

        public void InitElements(WeightedList<ItemEntry> pool)
        {
            List<ItemEntry> entries = new(pool.Keys);
            entries = entries.AsEnumerable()
                .Where((e) => e.Id != ItemID.None)
                .OrderBy((e) => e.Id)
                .OrderBy((e) => -(e.Min+e.Max))
                .OrderBy((e) => -pool[e])
                .ToList();
            
            SlotData = new SlotData[entries.Count];
            for (int n = 0; n < entries.Count; n++)
            {
                ItemEntry entry = entries[n];
                Item item = new(entry.Id);
                decimal chance = (decimal)(pool[entry] * 100 / pool.TotalWeight);
                SlotData data = new(item, entry.Min, entry.Max, chance);
                SlotData[n] = data;
            }
            scrollbar.SetView(Rows, MaxRows);
            UpdateSlots();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (scrollbar.ViewPosition != _previousViewPosition)
            {
                _previousViewPosition = scrollbar.ViewPosition;
                TopRow = (int)Math.Round(scrollbar.ViewPosition);
                UpdateSlots();
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

        private void UpdateSlots()
        {
            int offset = 0;
            for (int y = 0; y < Rows; y++)
            {
                for (int x = 0; x < Columns; x++)
                {
                    int slot = ((y + TopRow) * Columns) + x + offset;
                    while (slot < SlotData.Length && SlotData[slot].Item.type == ItemID.None)
                    {
                        offset++;
                        slot = ((y + TopRow) * Columns) + x + offset;
                    }
                    int min = 0, max = 0;
                    Item item = new();
                    if (slot<SlotData.Length)
                    {
                        item = SlotData[slot].Item;
                        min = SlotData[slot].Min;
                        max = SlotData[slot].Max;
                    }
                    UISystem uisys = ModContent.GetInstance<UISystem>();
                    Slots[y, x].SetItem(item, uisys is not null && !uisys.active);
                    Slots[y, x].SetAmount(min, max);
                }
            }
        }
    }
}
