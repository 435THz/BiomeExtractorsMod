using BiomeExtractorsMod.Common.Configs;
using BiomeExtractorsMod.Common.Systems;
using BiomeExtractorsMod.Content.TileEntities;
using BiomeExtractorsMod.Content.Tiles;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.Items
{
    class BiomeExtractorItemIron : BiomeExtractorItem
    {
        protected override int TileId => ModContent.TileType<BiomeExtractorTileBasic>();
        protected override ExtractorUpgradeKit UpgradeItemToCraftThis => throw new System.NotImplementedException(); //it should never be referenced anyway

        public override void SetDefaults()
        {
            base.SetDefaults();
            BiomeExtractionSystem.Instance.AddTier((int)BiomeExtractorEnt.EnumTiers.BASIC, $"{BiomeExtractorsMod.LocArticles}.Basic", $"{BiomeExtractorsMod.LocExtractorPrefix}Iron.DisplayName", delegate { return ConfigCommon.Instance.Tier1ExtractorRate; }, delegate { return ConfigCommon.Instance.Tier1ExtractorChance; }, "Content/MapIcons/BiomeExtractorIconBasic");
            Item.SetShopValues(ItemRarityColor.Blue1, Item.buyPrice(gold: 10)); // sell at 2
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Extractinator)
                .AddIngredient(ItemID.IronBar, 5)
                .AddIngredient(ItemID.Chain, 12)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}