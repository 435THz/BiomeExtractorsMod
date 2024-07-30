
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
        }


        public override void AddRecipes()
        {
            Recipe.Create(ModContent.ItemType<BiomeScanner>())
                .AddIngredient(ItemID.Lens, 2)
                .AddIngredient(ItemID.Wire, 5)
                .AddRecipeGroup(goldBarGroupName, 2)
                .AddTile(TileID.TinkerersWorkbench)
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