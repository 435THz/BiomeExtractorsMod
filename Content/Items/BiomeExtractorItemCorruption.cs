using BiomeExtractorsMod.Content.Tiles;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.Items
{
    class BiomeExtractorItemCorruption : BiomeExtractorItem
    {
        protected override int TileId => ModContent.TileType<BiomeExtractorTileDemonic>();
        protected override int TileStyle => 0; //TODO 1

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.SetShopValues(ItemRarityColor.Green2, Item.buyPrice(gold: 15)); // sell at 3
        }
    }
}