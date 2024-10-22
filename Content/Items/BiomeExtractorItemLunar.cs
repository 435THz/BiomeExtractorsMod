using BiomeExtractorsMod.Common.Configs;
using BiomeExtractorsMod.Common.Systems;
using BiomeExtractorsMod.Content.TileEntities;
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
            BiomeExtractionSystem.Instance.AddTier((int)BiomeExtractorEnt.EnumTiers.LUNAR, $"{BiomeExtractorsMod.LocArticles}.Lunar", $"{BiomeExtractorsMod.LocExtractorPrefix}Lunar.DisplayName", delegate { return ConfigCommon.Instance.Tier6ExtractorRate; }, delegate { return ConfigCommon.Instance.Tier6ExtractorChance; }, delegate { return Mod.Assets.Request<Texture2D>("Content/MapIcons/BiomeExtractorIconLunar"); });
            Item.SetShopValues(ItemRarityColor.Cyan9, Item.buyPrice(gold: 40)); // sell at 8
        }
    }
}