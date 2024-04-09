using BiomeExtractorsMod.Content.Tiles;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.Items
{
    class BiomeExtractorItemEthereal : BiomeExtractorItem
    {
        protected override int TileId => ModContent.TileType<BiomeExtractorTileEthereal>();

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.SetShopValues(ItemRarityColor.Purple11, Item.buyPrice(gold: 50)); // sell at 10
        }
    }
}