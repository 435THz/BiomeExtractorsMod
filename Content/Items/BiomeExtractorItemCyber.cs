using BiomeExtractorsMod.Common.Configs;
using BiomeExtractorsMod.Common.Systems;
using BiomeExtractorsMod.Content.TileEntities;
using BiomeExtractorsMod.Content.Tiles;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.Items
{
    public class BiomeExtractorItemCyber : BiomeExtractorItem
    {
        protected override int TileId => ModContent.TileType<BiomeExtractorTileCyber>();
        protected override ExtractorUpgradeKit UpgradeItemToCraftThis => ModContent.GetInstance<UpgradeKitCyber>();

        public override void SetDefaults()
        {
            base.SetDefaults();
            BiomeExtractionSystem.Instance.AddTier((int)BiomeExtractorEnt.EnumTiers.CYBER, $"{BiomeExtractorsMod.LocArticles}.Cyber", $"{BiomeExtractorsMod.LocExtractorPrefix}Cyber.DisplayName", delegate { return ConfigCommon.Instance.Tier5ExtractorRate; }, delegate { return ConfigCommon.Instance.Tier5ExtractorChance; }, "Content/MapIcons/BiomeExtractorIconCyber");
            Item.SetShopValues(ItemRarityColor.Lime7, Item.buyPrice(gold: 30)); // sell at 6
        }
    }
}