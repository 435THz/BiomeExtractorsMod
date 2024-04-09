using BiomeExtractorsMod.Content.Tiles;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.Items
{
    class BiomeExtractorItemIron : BiomeExtractorItem
    {
        protected override int TileId => ModContent.TileType<BiomeExtractorTileBasic>();

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.SetShopValues(ItemRarityColor.Blue1, Item.buyPrice(gold: 10)); // sell at 2
        }
    }
}