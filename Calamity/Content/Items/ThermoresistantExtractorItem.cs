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
    internal class ThermoresistantExtractorItem : BiomeExtractorItem
    {
        protected internal override int TileId => ModContent.TileType<ThermoresistantExtractorTile>();

        protected internal override ExtractorUpgradeKit UpgradeItemToCraftThis => throw new NotImplementedException();

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.SetShopValues(ItemRarityColor.LightRed4, Item.buyPrice(gold: 40)); // sell at 8
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<PressurizedExtractorItem>())
                .AddIngredient(ModContent.ItemType<ThermoresistantUpgradeKit>())
                .Register();
            
            Recipe quickRecipe = CreateRecipe()
                                    .AddIngredient(ModContent.ItemType<AbyssalUpgradeKit>())
                                    .AddIngredient(ModContent.ItemType<ThermoresistantUpgradeKit>())
                                    .AddIngredient(ModContent.ItemType<PressurizedUpgradeKit>())
                                    .AddIngredient(ModContent.ItemType<SulphuricUpgradeKit>());
            BuildRecipeFromTier(quickRecipe, BiomeExtractionSystem.Instance.GetTier(ExtractionTiers.DEMONIC));
            quickRecipe.Register();
        }
    }
}
