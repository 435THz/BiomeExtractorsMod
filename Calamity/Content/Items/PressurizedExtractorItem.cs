using System;
using BiomeExtractorsMod.Content.Items;
using BiomeExtractorsMod.Calamity.Content.Tiles;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;
using BiomeExtractorsMod.Common.Database;

namespace BiomeExtractorsMod.Calamity.Content.Items
{
    [ExtendsFromMod("CalamityMod")]
    [JITWhenModsEnabled("CalamityMod")]
    internal class PressurizedExtractorItem : BiomeExtractorItem
    {
        protected internal override int TileId => ModContent.TileType<PressurizedExtractorTile>();

        protected internal override ExtractorUpgradeKit UpgradeItemToCraftThis => throw new NotImplementedException();

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.SetShopValues(ItemRarityColor.LightRed4, Item.buyPrice(gold: 30)); // sell at 6
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<SulphuricExtractorItem>())
                .AddIngredient(ModContent.ItemType<PressurizedUpgradeKit>())
                .Register();
            
            Recipe quickRecipe = CreateRecipe()
                                    .AddIngredient(ModContent.ItemType<PressurizedUpgradeKit>())
                                    .AddIngredient(ModContent.ItemType<SulphuricUpgradeKit>());
            BuildRecipeFromTier(quickRecipe, BiomeExtractionSystem.Instance.GetTier(ExtractionTiers.DEMONIC));
            quickRecipe.Register();
        }
    }
}
