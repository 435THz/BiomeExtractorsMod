﻿using BiomeExtractorsMod.Common.Collections;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using static BiomeExtractorsMod.Common.Systems.BiomeExtractionSystem;

namespace BiomeExtractorsMod.Common.UI
{
    internal class UISlotArea : UIElement
    {
        private static readonly int Padding = 4;

        internal static int Columns = 8; //TODO make editable. Will probably require regenerating UISlotArea
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
            List<ItemEntry> entries = pool.Keys.ToList();
            entries.Sort(delegate(ItemEntry key1, ItemEntry key2) {
                int w1 = pool[key1], w2 = pool[key2];
                int a1 = key1.Min + key1.Max, a2 = key2.Min + key2.Max;
                if (w1 != w2)
                    return w2 - w1;
                else if (a1 != a2)
                    return a2 - a1;
                return key2.Id - key1.Id;
            });
            SlotData = new SlotData[entries.Count];
            for (int n = 0; n < entries.Count; n++)
            {
                ItemEntry entry = entries[n];
                Item item = new(entry.Id);
                decimal chance = pool[entry] * 100 / (decimal)pool.TotalWeight;
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
            for (int y = 0; y < Rows; y++)
            {
                for (int x = 0; x < Columns; x++)
                {
                    int slot = ((y + TopRow) * Columns) + x;
                    int min = 0, max = 0;
                    Item item = new();
                    if (slot<SlotData.Length)
                    {
                        item = SlotData[slot].Item;
                        min = SlotData[slot].Min;
                        max = SlotData[slot].Max;
                    }
                    UISystem uisys = ModContent.GetInstance<UISystem>();
                    Slots[y, x].SetItem(item, uisys is not null && !uisys.Extractor.Active);
                    Slots[y, x].SetAmount(min, max);
                }
            }
        }
    }
}
