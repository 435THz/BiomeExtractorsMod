using BiomeExtractorsMod.Common.Database;
using BiomeExtractorsMod.Content.Tiles;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.Items
{
    public class UpgradeKitTitanium : ExtractorUpgradeKit
    {
        protected override int Tier => ExtractionTiers.STEAMPUNK;
        protected override int ResultTile => ModContent.TileType<BiomeExtractorTileSteampunk>();
        protected override int TileStyle => 1;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.SetShopValues(ItemRarityColor.Pink5, Item.buyPrice(gold: 5)); // sell at 1
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.TitaniumBar, 5)
                .AddIngredient(ItemID.Cog, 12)
                .AddTile(TileID.MythrilAnvil) //covers both
                .Register();
        }
    }
}
