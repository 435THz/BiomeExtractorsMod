using BiomeExtractorsMod.Content.TileEntities;
using BiomeExtractorsMod.Content.Tiles;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.Items
{
    internal class UpgradeKitCyber : ExtractorUpgradeKit
    {
        protected override int Tier => (int)BiomeExtractorEnt.EnumTiers.CYBER;
        protected override int TileID => ModContent.TileType<BiomeExtractorTileCyber>();
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 18;
            Item.height = 18;
            Item.SetShopValues(ItemRarityColor.Lime7, Item.buyPrice(gold: 5)); // sell at 1
        }
    }
}
