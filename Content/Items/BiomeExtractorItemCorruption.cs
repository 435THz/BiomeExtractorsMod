using BiomeExtractorsMod.Common.Configs;
using BiomeExtractorsMod.Common.Database;
using BiomeExtractorsMod.Content.Tiles;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.Items
{
    public class BiomeExtractorItemCorruption : BiomeExtractorItem
    {
        protected internal override int TileId => ModContent.TileType<BiomeExtractorTileDemonic>();
        protected internal override ExtractorUpgradeKit UpgradeItemToCraftThis => ModContent.GetInstance<UpgradeKitCorruption>();

        public override void SetDefaults()
        {
            base.SetDefaults();
            BiomeExtractionSystem.Instance.AddTier(ExtractionTiers.DEMONIC, $"{BiomeExtractorsMod.LocArticles}.Demonic", $"{BiomeExtractorsMod.LocExtractorPrefix}Corruption.DisplayName", delegate { return ConfigCommon.Instance.Tier2ExtractorRate; }, delegate { return ConfigCommon.Instance.Tier2ExtractorChance; }, delegate { return ConfigCommon.Instance.Tier2ExtractorAmount; }, delegate { return Mod.Assets.Request<Texture2D>("Content/MapIcons/BiomeExtractorIconDemonic"); });
            Item.SetShopValues(ItemRarityColor.Green2, Item.buyPrice(gold: 15)); // sell at 3
        }
    }
}