using BiomeExtractorsMod.Content.Items;
using BiomeExtractorsMod.Calamity.Content.Tiles;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using BiomeExtractorsMod.Common.Database;

namespace BiomeExtractorsMod.Calamity.Content.Items
{
    [ExtendsFromMod("CalamityMod")]
    [JITWhenModsEnabled("CalamityMod")]
    internal class SpectralUpgradeKit : ExtractorUpgradeKit
    {
        protected override int Tier => ExtractionTiers.SPECTRAL;

        protected override int ResultTile => ModContent.TileType<SpectralExtractorTile>();

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.value = Item.buyPrice(gold: 10); // sell at 2
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<BloodstoneCore>(), 5)
                .AddIngredient(ModContent.ItemType<ReaperTooth>(), 12)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
