using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.UI;

namespace BiomeExtractorsMod.Common.UI
{
    internal struct SlotData(Item item, int min, int max, decimal chance)
    {

        internal Item Item = item;
        internal int Min = min;
        internal int Max = max;
        private decimal _chance = chance;
        internal decimal Chance {
            readonly get => decimal.Truncate(_chance * 100) / 100;
            set => _chance = value;
        }
        internal readonly double Med => (Min + Max) / 2.0;
        internal readonly int DailyAmount(double rollsPerDay) => (int)(rollsPerDay * Med * (double)Chance / 100);
        internal readonly string AmountString => Min == Max ? $"{Min}" : $"{Min}-{Max}";
        internal readonly string ChanceString => $"{Chance}{Language.GetTextValue($"{BiomeExtractorsMod.LocDiagnostics}.Percent")}";
        internal readonly string DailyString(double rollsPerDay) => ((int)DailyAmount(rollsPerDay)).ToString();
    }

    internal class UIResultSlot : UIPanel
    {
        internal static int size = 45;
        internal UIItemIcon icon;
        internal UIText amount;
        internal int index;
        public UIResultSlot(int index) : this(index, new Item(0), false) { }
        public UIResultSlot(int index, Item item, bool blackedOut) : base()
        {
            this.index = index;
            Width.Set(size, 0);
            Height.Set(size, 0);
            icon = new(item, blackedOut);
            icon.HAlign = icon.VAlign = 0.5f;
            Append(icon);

            amount = new("", 0.75f);
            amount.Top.Set(0f, 0.6f);
            amount.Left.Set(0f, -0.1f);
            Append(amount);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        internal void SetItem(Item item, bool blackedOut)
        {
            RemoveChild(amount);
            RemoveChild(icon);
            icon = new(item, blackedOut);
            Append(icon);
            Append(amount);
        }

        internal void SetAmount(int min, int max)
        {
            if (min == max)
            {
                if (min <= 1) amount.SetText("");
                else amount.SetText($"{min}");
            }
            else amount.SetText($"{min}-{max}");
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
            if (Parent is UISlotArea slotArea && slotArea.Hovered != index)
            {
                slotArea.Hovered = index;
            }
        }

        public override void MouseOut(UIMouseEvent evt)
        {
            base.MouseOut(evt);
            if (base.Parent is UISlotArea slotArea && slotArea.Hovered == index)
            {
                slotArea.Hovered = -1;
            }
        }
    }
}
