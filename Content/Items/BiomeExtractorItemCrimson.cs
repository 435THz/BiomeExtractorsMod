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
    public class BiomeExtractorItemCrimson : BiomeExtractorItem
    {
        protected internal override int TileId => ModContent.TileType<BiomeExtractorTileDemonic>();
        protected override int TileStyle => 1;
        protected override ExtractorUpgradeKit UpgradeItemToCraftThis => ModContent.GetInstance<UpgradeKitCrimson>();

        public override void SetDefaults()
        {
            base.SetDefaults();
            BiomeExtractionSystem.Instance.AddTier((int)BiomeExtractorEnt.EnumTiers.DEMONIC, $"{BiomeExtractorsMod.LocArticles}.Demonic", $"{BiomeExtractorsMod.LocExtractorPrefix}Corruption.DisplayName", delegate { return ConfigCommon.Instance.Tier2ExtractorRate; }, delegate { return ConfigCommon.Instance.Tier2ExtractorChance; }, delegate { return Mod.Assets.Request<Texture2D>("Content/MapIcons/BiomeExtractorIconDemonic"); });
            Item.SetShopValues(ItemRarityColor.Green2, Item.buyPrice(gold: 15)); // sell at 3
        }
    }
}