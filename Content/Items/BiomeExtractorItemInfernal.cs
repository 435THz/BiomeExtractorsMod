using BiomeExtractorsMod.Common.Configs;
using BiomeExtractorsMod.Common.Database;
using BiomeExtractorsMod.Content.Tiles;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.Items
{
    public class BiomeExtractorItemInfernal : BiomeExtractorItem
    {
        protected internal override int TileId => ModContent.TileType<BiomeExtractorTileInfernal>();
        protected internal override ExtractorUpgradeKit UpgradeItemToCraftThis => ModContent.GetInstance<UpgradeKitInfernal>();

        public override void SetDefaults()
        {
            base.SetDefaults();
            BiomeExtractionSystem.Instance.AddTier(ExtractionTiers.INFERNAL, $"{BiomeExtractorsMod.LocArticles}.Infernal", $"{BiomeExtractorsMod.LocExtractorPrefix}Infernal.DisplayName", delegate { return ConfigCommon.Instance.Tier3ExtractorRate; }, delegate { return ConfigCommon.Instance.Tier3ExtractorChance; }, delegate { return ConfigCommon.Instance.Tier3ExtractorAmount; }, delegate { return Mod.Assets.Request<Texture2D>("Content/MapIcons/BiomeExtractorIconInfernal"); });
            Item.SetShopValues(ItemRarityColor.Orange3, Item.buyPrice(gold: 20)); // sell at 4
        }
    }
}