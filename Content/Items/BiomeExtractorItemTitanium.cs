using BiomeExtractorsMod.Common.Configs;
using BiomeExtractorsMod.Common.Database;
using BiomeExtractorsMod.Content.Tiles;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.Items
{
    public class BiomeExtractorItemTitanium : BiomeExtractorItem
    {
        protected internal override int TileId => ModContent.TileType<BiomeExtractorTileSteampunk>();
        protected override int TileStyle => 1;
        protected override ExtractorUpgradeKit UpgradeItemToCraftThis => ModContent.GetInstance<UpgradeKitTitanium>();

        public override void SetDefaults()
        {
            base.SetDefaults();
            BiomeExtractionSystem.Instance.AddTier(ExtractionTiers.STEAMPUNK, $"{BiomeExtractorsMod.LocArticles}.Steampunk", $"{BiomeExtractorsMod.LocExtractorPrefix}Adamantite.DisplayName", delegate { return ConfigCommon.Instance.Tier4ExtractorRate; }, delegate { return ConfigCommon.Instance.Tier4ExtractorChance; }, delegate { return Mod.Assets.Request<Texture2D>("Content/MapIcons/BiomeExtractorIconSteampunk"); });
            Item.SetShopValues(ItemRarityColor.Pink5, Item.buyPrice(gold: 25)); // sell at 5
        }
    }
}