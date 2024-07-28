
using BiomeExtractorsMod.Content.Items;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Terraria.Localization;

namespace BiomeExtractorsMod.Common.Systems
{
    public class Recipes : ModSystem
    {
        readonly string goldBarGroupName =            nameof(ItemID.GoldBar);
        readonly string basicExtractorGroupName =     $"{nameof(BiomeExtractorsMod)}:{nameof(BiomeExtractorItemIron)}";
        readonly string demonicExtractorGroupName =   $"{nameof(BiomeExtractorsMod)}:{nameof(BiomeExtractorItemCorruption)}";
        readonly string steampunkExtractorGroupName = $"{nameof(BiomeExtractorsMod)}:{nameof(BiomeExtractorItemAdamantite)}";

        public override void AddRecipeGroups()
        {
            RecipeGroup goldBar = new(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.GoldBar)}", ItemID.GoldBar, ItemID.PlatinumBar);
            RecipeGroup.RegisterGroup(goldBarGroupName, goldBar);

            RecipeGroup basicExtractor = new(() => $"{Language.GetTextValue("LegacyMisc.37")} {Language.GetTextValue($"{BiomeExtractorsMod.LocExtractorPrefix}Iron.DisplayName")}", ModContent.ItemType<BiomeExtractorItemIron>(), ModContent.ItemType<BiomeExtractorItemLead>());
            RecipeGroup.RegisterGroup(basicExtractorGroupName, basicExtractor);

            RecipeGroup demonicExtractor = new(() => $"{Language.GetTextValue("LegacyMisc.37")} {Language.GetTextValue($"{BiomeExtractorsMod.LocExtractorPrefix}Corruption.DisplayName")}", ModContent.ItemType<BiomeExtractorItemCorruption>(), ModContent.ItemType<BiomeExtractorItemCrimson>());
            RecipeGroup.RegisterGroup(demonicExtractorGroupName, demonicExtractor);

            RecipeGroup steampunkExtractor = new(() => $"{Language.GetTextValue("LegacyMisc.37")} {Language.GetTextValue($"{BiomeExtractorsMod.LocExtractorPrefix}Adamantite.DisplayName")}", ModContent.ItemType<BiomeExtractorItemAdamantite>(), ModContent.ItemType<BiomeExtractorItemTitanium>());
            RecipeGroup.RegisterGroup(steampunkExtractorGroupName, steampunkExtractor);
        }


        public override void AddRecipes()
        {
            Recipe.Create(ModContent.ItemType<BiomeScanner>())
                .AddIngredient(ItemID.Lens, 2)
                .AddIngredient(ItemID.Wire, 5)
                .AddRecipeGroup(goldBarGroupName, 2)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();

            Recipe.Create(ModContent.ItemType<BiomeExtractorItemIron>())
                .AddIngredient(ItemID.Extractinator)
                .AddIngredient(ItemID.IronBar, 5)
                .AddIngredient(ItemID.Chain, 12)
                .AddTile(TileID.Anvils)
                .Register();

            Recipe.Create(ModContent.ItemType<BiomeExtractorItemLead>())
                .AddIngredient(ItemID.Extractinator)
                .AddIngredient(ItemID.LeadBar, 5)
                .AddIngredient(ItemID.Chain, 12)
                .AddTile(TileID.Anvils)
                .Register();

            Recipe.Create(ModContent.ItemType<BiomeExtractorItemCorruption>())
                .AddRecipeGroup(basicExtractorGroupName)
                .AddIngredient(ModContent.ItemType<UpgradeKitCorruption>())
                .Register();

            Recipe.Create(ModContent.ItemType<BiomeExtractorItemCrimson>())
                .AddRecipeGroup(basicExtractorGroupName)
                .AddIngredient(ModContent.ItemType<UpgradeKitCrimson>())
                .Register();

            Recipe.Create(ModContent.ItemType<BiomeExtractorItemInfernal>())
                .AddRecipeGroup(demonicExtractorGroupName)
                .AddIngredient(ModContent.ItemType<UpgradeKitInfernal>())
                .Register();

            Recipe.Create(ModContent.ItemType<BiomeExtractorItemAdamantite>())
                .AddIngredient(ModContent.ItemType<BiomeExtractorItemInfernal>())
                .AddIngredient(ModContent.ItemType<UpgradeKitAdamantite>())
                .Register();

            Recipe.Create(ModContent.ItemType<BiomeExtractorItemTitanium>())
                .AddIngredient(ModContent.ItemType<BiomeExtractorItemInfernal>())
                .AddIngredient(ModContent.ItemType<UpgradeKitTitanium>())
                .Register();

            Recipe.Create(ModContent.ItemType<BiomeExtractorItemCyber>())
                .AddRecipeGroup(steampunkExtractorGroupName)
                .AddIngredient(ModContent.ItemType<UpgradeKitCyber>())
                .Register();

            Recipe.Create(ModContent.ItemType<BiomeExtractorItemLunar>())
                .AddIngredient(ModContent.ItemType<BiomeExtractorItemCyber>())
                .AddIngredient(ModContent.ItemType<UpgradeKitLunar>())
                .Register();

            Recipe.Create(ModContent.ItemType<BiomeExtractorItemEthereal>())
                .AddIngredient(ModContent.ItemType<BiomeExtractorItemLunar>())
                .AddIngredient(ModContent.ItemType<UpgradeKitEthereal>())
                .Register();



            Recipe.Create(ModContent.ItemType<UpgradeKitCorruption>())
                .AddIngredient(ItemID.DemoniteBar, 5)
                .AddIngredient(ItemID.VilePowder, 12)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();

            Recipe.Create(ModContent.ItemType<UpgradeKitCrimson>())
                .AddIngredient(ItemID.CrimtaneBar, 5)
                .AddIngredient(ItemID.ViciousPowder, 12)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();

            Recipe.Create(ModContent.ItemType<UpgradeKitInfernal>())
                .AddIngredient(ItemID.HellstoneBar, 5)
                .AddIngredient(ItemID.Meteorite, 12)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();

            Recipe.Create(ModContent.ItemType<UpgradeKitAdamantite>())
                .AddIngredient(ItemID.AdamantiteBar, 5)
                .AddIngredient(ItemID.Cog, 12)
                .AddTile(TileID.MythrilAnvil) //covers both
                .Register();

            Recipe.Create(ModContent.ItemType<UpgradeKitTitanium>())
                .AddIngredient(ItemID.TitaniumBar, 5)
                .AddIngredient(ItemID.Cog, 12)
                .AddTile(TileID.MythrilAnvil) //covers both
                .Register();

            Recipe.Create(ModContent.ItemType<UpgradeKitCyber>())
                .AddIngredient(ItemID.ChlorophyteBar, 5)
                .AddIngredient(ItemID.Nanites, 12)
                .AddTile(TileID.MythrilAnvil)
                .Register();

            Recipe.Create(ModContent.ItemType<UpgradeKitLunar>())
                .AddIngredient(ItemID.FragmentNebula, 8)
                .AddIngredient(ItemID.FragmentSolar, 8)
                .AddIngredient(ItemID.FragmentStardust, 8)
                .AddIngredient(ItemID.FragmentVortex, 8)
                .AddTile(TileID.LunarCraftingStation)
                .AddCustomShimmerResult(ModContent.ItemType<UpgradeKitEthereal>())
                .AddDecraftCondition(Condition.DownedMoonLord)
                .Register();
        }
    }
}