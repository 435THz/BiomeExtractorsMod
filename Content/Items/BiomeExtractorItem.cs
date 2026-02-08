using BiomeExtractorsMod.Common.Database;
using BiomeExtractorsMod.Content.Tiles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.Items
{
    /// <summary>
    /// The core Item class implemented by all BiomeExtractors.
    /// </summary>
    public abstract class BiomeExtractorItem : ModItem
    {
        /// <summary>
        /// Returns the id of the BiomeExtractorTile this Item is bound to.
        /// </summary>
        protected internal abstract int TileId { get; }
        /// <summary>
        /// Returns the style index of the tile this Item creates.
        /// </summary>
        protected virtual int TileStyle => 0;

        /// <summary>
        /// Returns the ExtractorUpgradeKit required to craft this item.
        /// Feel free to completely ignore this value if you intend to override AddRecipes.
        /// </summary>
        protected internal abstract ExtractorUpgradeKit UpgradeItemToCraftThis { get; }

        private BiomeExtractionSystem.ExtractionTier Tier => BiomeExtractionSystem.Instance.GetTier(((BiomeExtractorTile)ModContent.GetModTile(TileId)).GetTileEntity.Tier);

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
            Tier.Register(this);
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(TileId, TileStyle);
        }

        public override void AddRecipes()
        {
            BiomeExtractionSystem.ExtractionTier t = UpgradeItemToCraftThis.LowerTier;
            Recipe recipe = CreateRecipe();
            if (t.Items.Count == 0)
            {
                throw new InvalidOperationException($"Tier \"{t.Name}\", tier number {t.Tier}, contains no items. Possible solutions:\n" +
                    "- Add items to this tier\n" +
                   $"- Override {GetType().Name}.AddRecipes() with a manually generated recipe\n" +
                    "- Remove the tier entirely");
            }
            if (t.Items.Count > 1)
                recipe.AddRecipeGroup(t.RecipeGroup); 
            else
                recipe.AddIngredient(t.Items[0]);
            recipe.AddIngredient(UpgradeItemToCraftThis);
            recipe.Register();
            
            Recipe quickRecipe = CreateRecipe();
            
            quickRecipe.AddIngredient(UpgradeItemToCraftThis);
            if(BuildRecipeFromTier(quickRecipe, UpgradeItemToCraftThis.LowerTier))
                quickRecipe.Register();
        }

        /// <summary>
        /// Adds to the given recipe all the extractor upgrade kits necessary to reach the given tier starting from basic, including the iron bar group, chains and
        /// extractinator, And sets anvils as its required crafting bench.
        /// </summary>
        /// <param name="recipe"> the recipe to add ingredients to. When this function is called by the default AddRecipes function, this recipe only has this
        /// object's "UpgradeItemToCraftThis" registered as ingredient.</param>
        /// <param name="tier"></param>
        /// <returns><see langword="true"/> if the recipe could be built, false otherwise.</returns>
        protected virtual bool BuildRecipeFromTier(Recipe recipe, BiomeExtractionSystem.ExtractionTier tier)
        {
            while (tier.Tier > ExtractionTiers.BASIC)
            {
                List<BiomeExtractorItem> items = tier.Items;
                if(items.Count==0)
                {
                    Console.Error.Write($"Could not generate a Quick Recipe for {GetType().Name}\n"+
                        $"Tier \"{tier.Name}\", tier number {tier.Tier}, contains no items.\n"+
                        $"If you want to set a custom Quick Recipe, please override {GetType().Name}.BuildRecipeFromTier(Recipe, ExtractionTier) with your implementation and make sure it returns true\n" +
                        $"If you do not want a Quick Recipe, please override {GetType().Name}.BuildRecipeFromTier(Recipe, ExtractionTier) with your implementation and make sure it returns false");
                    return false;
                }
                if(tier.KitRecipeGroup==null)
                {
                    Console.Error.Write($"Could not generate a Quick Recipe for {GetType().Name}\n"+
                        $"Tier\"{tier.Name}\", tier number {tier.Tier}, is associated to items whose UpgradeItemToCraftThis property is null.\n" +
                        $"If you want to set a custom Quick Recipe, please override {GetType().Name}.BuildRecipeFromTier(Recipe, ExtractionTier) with your implementation and make sure it returns true\n" +
                        $"If you do not want a Quick Recipe, please override {GetType().Name}.BuildRecipeFromTier(Recipe, ExtractionTier) with your implementation and make sure it returns false");
                    return false;
                }
                if(items.Count>1)
                    recipe.AddRecipeGroup(tier.KitRecipeGroup);
                else
                    recipe.AddIngredient(items[0].UpgradeItemToCraftThis);
                tier = BiomeExtractionSystem.Instance.GetClosestLowerTier(tier.Tier);
            }
            recipe.AddIngredient(ItemID.Chain, 12);
            recipe.AddRecipeGroup(RecipeGroupID.IronBar, 5);
            recipe.AddIngredient(ItemID.Extractinator);
            recipe.AddTile(TileID.Anvils);
            recipe.requiredItem.Reverse();
            return true;
        }
    }
}