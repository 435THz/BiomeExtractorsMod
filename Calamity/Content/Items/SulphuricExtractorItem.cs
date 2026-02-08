using System;
using BiomeExtractorsMod.Common.Database;
using BiomeExtractorsMod.Content.Items;
using BiomeExtractorsMod.Calamity.Content.Tiles;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Calamity.Content.Items
{
    [ExtendsFromMod("CalamityMod")]
    [JITWhenModsEnabled("CalamityMod")]
    internal class SulphuricExtractorItem : BiomeExtractorItem
    {
        protected internal override int TileId => ModContent.TileType<SulphuricExtractorTile>();

        protected internal override ExtractorUpgradeKit UpgradeItemToCraftThis => throw new NotImplementedException();

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.SetShopValues(ItemRarityColor.Orange3, Item.buyPrice(gold: 22)); // sell at 4.4
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup(Recipes.demonicExtractorGroupName)
                .AddIngredient(ModContent.ItemType<SulphuricUpgradeKit>())
                .Register();
        }
    }
}
