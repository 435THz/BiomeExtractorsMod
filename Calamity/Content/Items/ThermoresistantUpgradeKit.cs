using BiomeExtractorsMod.Content.Items;
using BiomeExtractorsMod.Calamity.Content.Tiles;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Calamity.Content.Items
{
    [ExtendsFromMod("CalamityMod")]
    [JITWhenModsEnabled("CalamityMod")]
    internal class ThermoresistantUpgradeKit : ExtractorModificationKit
    {
        protected override int[] TargetTiles => [ModContent.TileType<PressurizedExtractorTile>()];

        protected override int ResultTile => ModContent.TileType<ThermoresistantExtractorTile>();

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.SetShopValues(ItemRarityColor.Yellow8, Item.buyPrice(gold: 10)); // sell at 2
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<ScoriaBar>(), 5)
                .AddIngredient(ItemID.LihzahrdPowerCell, 6)
                .AddIngredient(ModContent.ItemType<PlagueCellCanister>(), 6)
                .AddTile(TileID.MythrilAnvil) //covers both
                .Register();
        }
    }
}
