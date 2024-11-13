using BiomeExtractorsMod.Common.Configs;
using BiomeExtractorsMod.Common.Database;
using BiomeExtractorsMod.Content.Tiles;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.Items
{
    public class BiomeExtractorItemEthereal : BiomeExtractorItem
    {
        protected internal override int TileId => ModContent.TileType<BiomeExtractorTileEthereal>();
        protected override ExtractorUpgradeKit UpgradeItemToCraftThis => ModContent.GetInstance<UpgradeKitEthereal>();

        public override void SetDefaults()
        {
            base.SetDefaults();
            BiomeExtractionSystem.Instance.AddTier(ExtractionTiers.ETHEREAL, $"{BiomeExtractorsMod.LocArticles}.Ethereal", $"{BiomeExtractorsMod.LocExtractorPrefix}Ethereal.DisplayName", delegate { return ConfigCommon.Instance.Tier7ExtractorRate; }, delegate { return ConfigCommon.Instance.Tier7ExtractorChance; }, delegate { return Mod.Assets.Request<Texture2D>("Content/MapIcons/BiomeExtractorIconEthereal"); });
            Item.SetShopValues(ItemRarityColor.Purple11, Item.buyPrice(gold: 50)); // sell at 10
        }
    }
}