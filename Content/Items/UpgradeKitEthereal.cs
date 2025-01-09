using BiomeExtractorsMod.Common.Database;
using BiomeExtractorsMod.Content.Tiles;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.Items
{
    public class UpgradeKitEthereal : ExtractorUpgradeKit
    {
        protected override int Tier => ExtractionTiers.ETHEREAL;
        protected override int ResultTile => ModContent.TileType<BiomeExtractorTileEthereal>();
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 22;
            Item.height = 20;
            Item.SetShopValues(ItemRarityColor.Purple11, Item.buyPrice(gold: 10)); // sell at 2
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.LunarBar, 5)
                .AddIngredient(ItemID.CrystalShard, 24)
                .AddIngredient(ItemID.GalaxyPearl)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
