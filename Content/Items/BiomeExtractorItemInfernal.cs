using BiomeExtractorsMod.Content.Tiles;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.Items
{
    class BiomeExtractorItemInfernal : BiomeExtractorItem
    {
        protected override int GetTileId()
        {
            return ModContent.TileType<BiomeExtractorTileInfernal>();
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.SetShopValues(ItemRarityColor.Orange3, Item.buyPrice(gold: 20)); // sell at 4
        }
    }
}