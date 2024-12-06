using BiomeExtractorsMod.Common.Configs;
using BiomeExtractorsMod.Common.Database;
using BiomeExtractorsMod.Content.Tiles;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.Items
{
    public class BiomeExtractorItemLead : BiomeExtractorItem
    {
        protected internal override int TileId => ModContent.TileType<BiomeExtractorTileBasic>();
        protected override int TileStyle => 1;
        protected override ExtractorUpgradeKit UpgradeItemToCraftThis => throw new System.NotImplementedException(); //it should never be referenced anyway

        public override void SetDefaults()
        {
            base.SetDefaults();
            BiomeExtractionSystem.Instance.AddTier(ExtractionTiers.BASIC, $"{BiomeExtractorsMod.LocArticles}.Basic", $"{BiomeExtractorsMod.LocExtractorPrefix}Iron.DisplayName", delegate { return ConfigCommon.Instance.Tier1ExtractorRate; }, delegate { return ConfigCommon.Instance.Tier1ExtractorChance; }, delegate { return ConfigCommon.Instance.Tier1ExtractorAmount; }, delegate { return Mod.Assets.Request<Texture2D>("Content/MapIcons/BiomeExtractorIconBasic"); });
            Item.SetShopValues(ItemRarityColor.Blue1, Item.buyPrice(gold: 10)); // sell at 2
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Extractinator)
                .AddIngredient(ItemID.LeadBar, 5)
                .AddIngredient(ItemID.Chain, 12)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}