using BiomeExtractorsMod.Common.Database;
using BiomeExtractorsMod.Content.Tiles;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.Items
{
    public class UpgradeKitLunar : ExtractorUpgradeKit
    {
        protected override int Tier => ExtractionTiers.LUNAR;
        protected override int ResultTile => ModContent.TileType<BiomeExtractorTileLunar>();
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 16;
            Item.height = 16;
            Item.SetShopValues(ItemRarityColor.Cyan9, Item.buyPrice(gold: 10)); // sell at 2
        }
    }
}
