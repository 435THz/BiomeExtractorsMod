using BiomeExtractorsMod.Content.Tiles;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.Items
{
    class BiomeExtractorItemTitanium : BiomeExtractorItem
    {
        protected override int TileId => ModContent.TileType<BiomeExtractorTileSteampunk>();
        protected override int TileStyle => 1;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.SetShopValues(ItemRarityColor.Pink5, Item.buyPrice(gold: 25)); // sell at 5
        }
    }
}