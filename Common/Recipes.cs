
using BiomeExtractorsMod.Content.Items;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Terraria.Localization;

namespace BiomeExtractorsMod.Common
{
    public class Recipes : ModSystem
    {
        public override void AddRecipeGroups()
        {
            RecipeGroup evilOres = new(() => $"{Language.GetTextValue("LegacyMisc.37")} {Language.GetTextValue($"{BiomeExtractorsMod.LocItemGroups}.EvilBars")}", ItemID.DemoniteBar, ItemID.CrimtaneBar);
            RecipeGroup.RegisterGroup(nameof(ItemID.DemoniteBar), evilOres);

            RecipeGroup evilSamples = new(() => $"{Language.GetTextValue("LegacyMisc.37")} {Language.GetTextValue($"{BiomeExtractorsMod.LocItemGroups}.EvilSamples")}", ItemID.ShadowScale, ItemID.TissueSample);
            RecipeGroup.RegisterGroup(nameof(ItemID.ShadowScale), evilSamples);

            RecipeGroup hmOres3 = new(() => $"{Language.GetTextValue("LegacyMisc.37")} {Language.GetTextValue($"{BiomeExtractorsMod.LocItemGroups}.Tier3HM")}", ItemID.AdamantiteBar, ItemID.TitaniumBar);
            RecipeGroup.RegisterGroup(nameof(ItemID.AdamantiteBar), hmOres3);
        }


        public override void AddRecipes()
        {
            Recipe.Create(ModContent.ItemType<BiomeExtractorItemBasic>())
                .AddIngredient(ItemID.Extractinator)
                .AddRecipeGroup(RecipeGroupID.IronBar, 5)
                .AddIngredient(ItemID.Chain, 12)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();

            Recipe.Create(ModContent.ItemType<BiomeExtractorItemDemonic>())
                .AddIngredient(ModContent.ItemType<BiomeExtractorItemBasic>())
                .AddRecipeGroup(nameof(ItemID.DemoniteBar), 5)
                .AddRecipeGroup(nameof(ItemID.ShadowScale), 12)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();

            Recipe.Create(ModContent.ItemType<BiomeExtractorItemInfernal>())
                .AddIngredient(ModContent.ItemType<BiomeExtractorItemDemonic>())
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