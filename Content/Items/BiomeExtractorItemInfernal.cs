using BiomeExtractorsMod.Common.Configs;
using BiomeExtractorsMod.Common.Systems;
using BiomeExtractorsMod.Content.TileEntities;
using BiomeExtractorsMod.Content.Tiles;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.Items
{
    class BiomeExtractorItemInfernal : BiomeExtractorItem
    {
        protected override int TileId => ModContent.TileType<BiomeExtractorTileInfernal>();

        public override void SetDefaults()
        {
            base.SetDefaults();
            BiomeExtractionSystem.Instance.AddTier((int)BiomeExtractorEnt.EnumTiers.INFERNAL, $"{BiomeExtractorsMod.LocArticles}.Infernal", $"{BiomeExtractorsMod.LocExtractorPrefix}Infernal.DisplayName", delegate { return ConfigCommon.Instance.Tier3ExtractorRate; }, delegate { return ConfigCommon.Instance.Tier3ExtractorChance; }, "Content/MapIcons/BiomeExtractorIconInfernal");
            Item.SetShopValues(ItemRarityColor.Orange3, Item.buyPrice(gold: 20)); // sell at 4
        }
    }
}