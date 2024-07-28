using BiomeExtractorsMod.Content.TileEntities;
using BiomeExtractorsMod.Content.Tiles;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.Items
{
    internal class UpgradeKitTitanium : ExtractorUpgradeKit
    {
        protected override int Tier => (int)BiomeExtractorEnt.EnumTiers.STEAMPUNK;
        protected override int TileID => ModContent.TileType<BiomeExtractorTileSteampunk>();
        protected override int TileStyle => 1;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.SetShopValues(ItemRarityColor.Pink5, Item.buyPrice(gold: 5)); // sell at 1
        }
    }
}
