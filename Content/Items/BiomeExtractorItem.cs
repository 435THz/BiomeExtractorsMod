using BiomeExtractorsMod.Common.Systems;
using BiomeExtractorsMod.Content.Tiles;
using System;
using Terraria;
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
        protected abstract int TileId { get; }
        /// <summary>
        /// Returns the style index of the tile this Item creates.
        /// </summary>
        protected virtual int TileStyle => 0;

        /// <summary>
        /// Returns the ExtractorUpgradeKit required to craft this item.
        /// Feel free to completely ignore this value if you intend to override AddRecipes.
        /// </summary>
        protected abstract ExtractorUpgradeKit UpgradeItemToCraftThis { get; }

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
                throw new InvalidOperationException($"The Tier \"{t.Name}\", tier number {t.Tier}, contains no items. Possible solutions:" +
                    "- Add items to this tier" +
                   $"- Override {GetType().Name}.AddRecipes() with a manually generated recipe" +
                    "- Remove the tier entirely");
            }
            if (t.Items.Count > 1)
                recipe.AddRecipeGroup(t.RecipeGroup); 
            else
                recipe.AddIngredient(t.Items[0]);
            recipe.AddIngredient(UpgradeItemToCraftThis);
            recipe.Register();
        }
    }
}