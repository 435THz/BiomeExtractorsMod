using BiomeExtractorsMod.Content.Items;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace BiomeExtractorsMod
{
	public class BiomeExtractorsMod : Mod
	{ }

    public class BESys : ModSystem
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