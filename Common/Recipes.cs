
using BiomeExtractorsMod.Content.Items;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace BiomeExtractorsMod.Common
{
    public class Recipes : ModSystem
    {
        public override void AddRecipes()
        {
            Recipe.Create(ModContent.ItemType<BiomeExtractorItemBasic>())
                .AddIngredient(ItemID.Extractinator)
                .AddRecipeGroup(RecipeGroupID.IronBar, 4)
                .AddIngredient(ItemID.Wire, 10)
                .AddIngredient(ItemID.Chain, 10)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}