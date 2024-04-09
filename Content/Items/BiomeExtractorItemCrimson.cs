using BiomeExtractorsMod.Content.Tiles;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.Items
{
    class BiomeExtractorItemCrimson : BiomeExtractorItem
    {
        protected override int TileId => ModContent.TileType<BiomeExtractorTileDemonic>();

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.SetShopValues(ItemRarityColor.Green2, Item.buyPrice(gold: 15)); // sell at 3
        }
    }
}