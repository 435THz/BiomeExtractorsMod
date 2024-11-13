using BiomeExtractorsMod.Common.Database;
using BiomeExtractorsMod.Content.Tiles;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.Items
{
    public class UpgradeKitInfernal : ExtractorUpgradeKit
    {
        protected override int Tier => ExtractionTiers.INFERNAL;
        protected override int ResultTile => ModContent.TileType<BiomeExtractorTileInfernal>();
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.SetShopValues(ItemRarityColor.Orange3, Item.buyPrice(gold: 5)); // sell at 1
        }
    }
}
