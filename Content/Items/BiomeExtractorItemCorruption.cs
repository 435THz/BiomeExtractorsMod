using BiomeExtractorsMod.Common.Configs;
using BiomeExtractorsMod.Common.Systems;
using BiomeExtractorsMod.Content.TileEntities;
using BiomeExtractorsMod.Content.Tiles;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.Items
{
    class BiomeExtractorItemCorruption : BiomeExtractorItem
    {
        protected override int TileId => ModContent.TileType<BiomeExtractorTileDemonic>();
        protected override ExtractorUpgradeKit UpgradeItemToCraftThis => ModContent.GetInstance<UpgradeKitCorruption>();

        public override void SetDefaults()
        {
            base.SetDefaults();
            BiomeExtractionSystem.Instance.AddTier((int)BiomeExtractorEnt.EnumTiers.DEMONIC, $"{BiomeExtractorsMod.LocArticles}.Demonic", $"{BiomeExtractorsMod.LocExtractorPrefix}Corruption.DisplayName", delegate { return ConfigCommon.Instance.Tier2ExtractorRate; }, delegate { return ConfigCommon.Instance.Tier2ExtractorChance; }, "Content/MapIcons/BiomeExtractorIconDemonic");
            Item.SetShopValues(ItemRarityColor.Green2, Item.buyPrice(gold: 15)); // sell at 3
        }
    }
}