using BiomeExtractorsMod.Common.Configs;
using BiomeExtractorsMod.Common.Database;
using BiomeExtractorsMod.Content.Tiles;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.Items
{
    public class BiomeExtractorItemAdamantite : BiomeExtractorItem
    {
        protected internal override int TileId => ModContent.TileType<BiomeExtractorTileSteampunk>();
        protected internal override ExtractorUpgradeKit UpgradeItemToCraftThis => ModContent.GetInstance<UpgradeKitAdamantite>();

        public override void SetDefaults()
        {
            base.SetDefaults();
            BiomeExtractionSystem.Instance.AddTier((int)ExtractionTiers.STEAMPUNK, $"{BiomeExtractorsMod.LocArticles}.Steampunk", $"{BiomeExtractorsMod.LocExtractorPrefix}Adamantite.DisplayName", delegate { return ConfigCommon.Instance.Tier4ExtractorRate; }, delegate { return ConfigCommon.Instance.Tier4ExtractorChance; }, delegate { return ConfigCommon.Instance.Tier4ExtractorAmount; }, delegate { return Mod.Assets.Request<Texture2D>("Content/MapIcons/BiomeExtractorIconSteampunk"); });
            Item.SetShopValues(ItemRarityColor.Pink5, Item.buyPrice(gold: 25)); // sell at 5
        }
    }
}