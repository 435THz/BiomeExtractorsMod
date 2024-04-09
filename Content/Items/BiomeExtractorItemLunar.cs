using BiomeExtractorsMod.Content.Tiles;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.Items
{
    class BiomeExtractorItemLunar : BiomeExtractorItem
    {
        protected override int TileId => ModContent.TileType<BiomeExtractorTileLunar>();

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.SetShopValues(ItemRarityColor.Cyan9, Item.buyPrice(gold: 40)); // sell at 8
        }
    }
}