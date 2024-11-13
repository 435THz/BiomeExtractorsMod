using BiomeExtractorsMod.Common.Database;
using BiomeExtractorsMod.Content.Tiles;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.Items
{
    public class UpgradeKitEthereal : ExtractorUpgradeKit
    {
        protected override int Tier => ExtractionTiers.ETHEREAL;
        protected override int ResultTile => ModContent.TileType<BiomeExtractorTileEthereal>();
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 22;
            Item.height = 20;
            Item.SetShopValues(ItemRarityColor.Purple11, Item.buyPrice(gold: 10)); // sell at 2
        }
    }
}
