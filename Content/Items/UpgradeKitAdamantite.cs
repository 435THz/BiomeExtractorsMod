using BiomeExtractorsMod.Common.Database;
using BiomeExtractorsMod.Content.Tiles;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.Items
{
    public class UpgradeKitAdamantite : ExtractorUpgradeKit
    {
        protected override int Tier => ExtractionTiers.STEAMPUNK;
        protected override int ResultTile => ModContent.TileType<BiomeExtractorTileSteampunk>();
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.SetShopValues(ItemRarityColor.Pink5, Item.buyPrice(gold: 5)); // sell at 1
        }
    }
}
