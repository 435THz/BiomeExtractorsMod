using BiomeExtractorsMod.Common.Configs;
using BiomeExtractorsMod.Common.Database;
using BiomeExtractorsMod.Content.Tiles;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.Items
{
    public class BiomeExtractorItemCyber : BiomeExtractorItem
    {
        protected internal override int TileId => ModContent.TileType<BiomeExtractorTileCyber>();
        protected override ExtractorUpgradeKit UpgradeItemToCraftThis => ModContent.GetInstance<UpgradeKitCyber>();

        public override void SetDefaults()
        {
            base.SetDefaults();
            BiomeExtractionSystem.Instance.AddTier(ExtractionTiers.CYBER, $"{BiomeExtractorsMod.LocArticles}.Cyber", $"{BiomeExtractorsMod.LocExtractorPrefix}Cyber.DisplayName", delegate { return ConfigCommon.Instance.Tier5ExtractorRate; }, delegate { return ConfigCommon.Instance.Tier5ExtractorChance; }, delegate { return Mod.Assets.Request<Texture2D>("Content/MapIcons/BiomeExtractorIconCyber"); });
            Item.SetShopValues(ItemRarityColor.Lime7, Item.buyPrice(gold: 30)); // sell at 6
        }
    }
}