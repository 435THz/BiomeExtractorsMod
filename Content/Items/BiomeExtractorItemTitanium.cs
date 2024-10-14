using BiomeExtractorsMod.Common.Configs;
using BiomeExtractorsMod.Common.Systems;
using BiomeExtractorsMod.Content.TileEntities;
using BiomeExtractorsMod.Content.Tiles;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.Items
{
    public class BiomeExtractorItemTitanium : BiomeExtractorItem
    {
        protected override int TileId => ModContent.TileType<BiomeExtractorTileSteampunk>();
        protected override int TileStyle => 1;
        protected override ExtractorUpgradeKit UpgradeItemToCraftThis => ModContent.GetInstance<UpgradeKitTitanium>();

        public override void SetDefaults()
        {
            base.SetDefaults();
            BiomeExtractionSystem.Instance.AddTier((int)BiomeExtractorEnt.EnumTiers.STEAMPUNK, $"{BiomeExtractorsMod.LocArticles}.Steampunk", $"{BiomeExtractorsMod.LocExtractorPrefix}Adamantite.DisplayName", delegate { return ConfigCommon.Instance.Tier4ExtractorRate; }, delegate { return ConfigCommon.Instance.Tier4ExtractorChance; }, "Content/MapIcons/BiomeExtractorIconSteampunk");
            Item.SetShopValues(ItemRarityColor.Pink5, Item.buyPrice(gold: 25)); // sell at 5
        }
    }
}