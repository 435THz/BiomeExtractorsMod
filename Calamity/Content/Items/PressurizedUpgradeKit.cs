using BiomeExtractorsMod.Content.Items;
using BiomeExtractorsMod.Calamity.Content.Tiles;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Placeables.FurnitureAbyss;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Calamity.Content.Items
{
    [ExtendsFromMod("CalamityMod")]
    [JITWhenModsEnabled("CalamityMod")]
    internal class PressurizedUpgradeKit : ExtractorModificationKit
    {
        protected override int[] TargetTiles => [ModContent.TileType<SulphuricExtractorTile>()];

        protected override int ResultTile => ModContent.TileType<PressurizedExtractorTile>();

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.SetShopValues(ItemRarityColor.LightRed4, Item.buyPrice(gold: 8)); // sell at 1.6
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.HellstoneBar, 5)
                .AddIngredient(ItemID.GoldenKey, 6)
                .AddIngredient(ModContent.ItemType<PlantyMush>(), 6)
                .AddIngredient(ModContent.ItemType<SmoothAbyssGravel>(), 6)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}
