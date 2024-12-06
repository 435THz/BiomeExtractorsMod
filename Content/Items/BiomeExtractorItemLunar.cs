using BiomeExtractorsMod.Common.Configs;
using BiomeExtractorsMod.Common.Database;
using BiomeExtractorsMod.Content.Tiles;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.Items
{
    public class BiomeExtractorItemLunar : BiomeExtractorItem
    {
        protected internal override int TileId => ModContent.TileType<BiomeExtractorTileLunar>();
        protected override ExtractorUpgradeKit UpgradeItemToCraftThis => ModContent.GetInstance<UpgradeKitLunar>();

        public override void SetDefaults()
        {
            base.SetDefaults();
            BiomeExtractionSystem.Instance.AddTier(ExtractionTiers.LUNAR, $"{BiomeExtractorsMod.LocArticles}.Lunar", $"{BiomeExtractorsMod.LocExtractorPrefix}Lunar.DisplayName", delegate { return ConfigCommon.Instance.Tier6ExtractorRate; }, delegate { return ConfigCommon.Instance.Tier6ExtractorChance; }, delegate { return ConfigCommon.Instance.Tier6ExtractorAmount; }, delegate { return Mod.Assets.Request<Texture2D>("Content/MapIcons/BiomeExtractorIconLunar"); });
            Item.SetShopValues(ItemRarityColor.Cyan9, Item.buyPrice(gold: 40)); // sell at 8
        }
    }
}