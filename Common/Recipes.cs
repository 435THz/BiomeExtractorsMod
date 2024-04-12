
using BiomeExtractorsMod.Content.Items;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Terraria.Localization;

namespace BiomeExtractorsMod.Common
{
    public class Recipes : ModSystem
    {
        readonly string basicExtractorGroupName = $"{nameof(BiomeExtractorsMod)}:{nameof(BiomeExtractorItemIron)}";
        readonly string demonicExtractorGroupName = $"{nameof(BiomeExtractorsMod)}:{nameof(BiomeExtractorItemCorruption)}";

        public override void AddRecipeGroups()
        {
            RecipeGroup basicExtractor = new(() => $"{Language.GetTextValue("LegacyMisc.37")} {Language.GetTextValue($"{BiomeExtractorsMod.LocItemGroups}.BasicExtractor")}", ModContent.ItemType<BiomeExtractorItemIron>(), ModContent.ItemType<BiomeExtractorItemLead>());
            RecipeGroup.RegisterGroup(basicExtractorGroupName, basicExtractor);

            RecipeGroup demonicExtractor = new(() => $"{Language.GetTextValue("LegacyMisc.37")} {Language.GetTextValue($"{BiomeExtractorsMod.LocItemGroups}.DemonicExtractor")}", ModContent.ItemType<BiomeExtractorItemCorruption>(), ModContent.ItemType<BiomeExtractorItemCrimson>());
            RecipeGroup.RegisterGroup(demonicExtractorGroupName, demonicExtractor);

            RecipeGroup hmOres3 = new(() => $"{Language.GetTextValue("LegacyMisc.37")} {Language.GetTextValue($"{BiomeExtractorsMod.LocItemGroups}.Tier3HM")}", ItemID.AdamantiteBar, ItemID.TitaniumBar);
            RecipeGroup.RegisterGroup(nameof(ItemID.AdamantiteBar), hmOres3);
        }


        public override void AddRecipes()
        {
            Recipe.Create(ModContent.ItemType<BiomeExtractorItemIron>())
                .AddIngredient(ItemID.Extractinator)
                .AddIngredient(ItemID.IronBar, 5)
                .AddIngredient(ItemID.Chain, 12)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();

            Recipe.Create(ModContent.ItemType<BiomeExtractorItemLead>())
                .AddIngredient(ItemID.Extractinator)
                .AddIngredient(ItemID.LeadBar, 5)
                .AddIngredient(ItemID.Chain, 12)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();

            Recipe.Create(ModContent.ItemType<BiomeExtractorItemCorruption>())
                .AddRecipeGroup(basicExtractorGroupName)
                .AddIngredient(ItemID.DemoniteBar, 5)
                .AddIngredient(ItemID.ShadowScale, 12)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();

            Recipe.Create(ModContent.ItemType<BiomeExtractorItemCrimson>())
                .AddRecipeGroup(basicExtractorGroupName)
                .AddIngredient(ItemID.CrimtaneBar, 5)
                .AddIngredient(ItemID.TissueSample, 12)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();

            Recipe.Create(ModContent.ItemType<BiomeExtractorItemInfernal>())
                .AddRecipeGroup(demonicExtractorGroupName)
                .AddIngredient(ItemID.HellstoneBar, 5)
                .AddIngredient(ItemID.Meteorite, 12)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();

            Recipe.Create(ModContent.ItemType<BiomeExtractorItemSteampunk>())
                .AddIngredient(ModContent.ItemType<BiomeExtractorItemInfernal>())
                .AddRecipeGroup(nameof(ItemID.AdamantiteBar), 5)
                .AddIngredient(ItemID.Cog, 12)
                .AddTile(TileID.MythrilAnvil) //covers both
                .Register();

            Recipe.Create(ModContent.ItemType<BiomeExtractorItemCyber>())
                .AddIngredient(ModContent.ItemType<BiomeExtractorItemSteampunk>())
                .AddIngredient(ItemID.ChlorophyteBar, 5)
                .AddIngredient(ItemID.Nanites, 12)
                .AddTile(TileID.MythrilAnvil)
                .Register();

            Recipe.Create(ModContent.ItemType<BiomeExtractorItemLunar>())
                .AddIngredient(ModContent.ItemType<BiomeExtractorItemCyber>())
                .AddIngredient(ItemID.FragmentNebula, 8)
                .AddIngredient(ItemID.FragmentSolar, 8)
                .AddIngredient(ItemID.FragmentStardust, 8)
                .AddIngredient(ItemID.FragmentVortex, 8)
                .AddTile(TileID.LunarCraftingStation)
                .AddCustomShimmerResult(ModContent.ItemType<BiomeExtractorItemEthereal>())
                .AddDecraftCondition(Condition.DownedMoonLord)
                .Register();
        }
    }
}