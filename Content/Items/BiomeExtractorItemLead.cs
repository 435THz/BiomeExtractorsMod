using BiomeExtractorsMod.Content.Tiles;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.Items
{
    class BiomeExtractorItemLead : BiomeExtractorItem
    {
        protected override int TileId => ModContent.TileType<BiomeExtractorTileBasic>();
        protected override int TileStyle => 1;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.SetShopValues(ItemRarityColor.Blue1, Item.buyPrice(gold: 10)); // sell at 2
        }
    }
}