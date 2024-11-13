using BiomeExtractorsMod.Common.Database;
using BiomeExtractorsMod.Content.Tiles;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.Items
{
    public class UpgradeKitCrimson : ExtractorUpgradeKit
    {
        protected override int Tier => ExtractionTiers.DEMONIC;
        protected override int ResultTile => ModContent.TileType<BiomeExtractorTileDemonic>();
        protected override int TileStyle => 1;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.SetShopValues(ItemRarityColor.Green2, Item.buyPrice(gold: 5)); // sell at 1
        }
    }
}
