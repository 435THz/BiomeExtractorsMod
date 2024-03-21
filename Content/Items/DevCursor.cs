using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.Items
{
    class DevCursor : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 1;
            Item.value = 0;
            Item.rare = ItemRarityID.Red;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.maxStack = 1;
            Item.useTurn = true;
        }

        public override bool? UseItem(Player player)
        {
            Point pos = Main.MouseWorld.ToTileCoordinates();
            Main.NewText("Cursor: ("+pos.X+", "+pos.Y+")");
            return true;
        }
    }
}