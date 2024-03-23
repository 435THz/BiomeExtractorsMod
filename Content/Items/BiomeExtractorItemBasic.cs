using BiomeExtractorsMod.Content.Tiles;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.Items
{
    class BiomeExtractorItemBasic : BiomeExtractorItem
    {
        protected override int GetTileId()
        {
            return ModContent.TileType<BiomeExtractorTileBasic>();
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.SetShopValues(ItemRarityColor.Green2, Item.buyPrice(gold: 15));
        }
    }
}