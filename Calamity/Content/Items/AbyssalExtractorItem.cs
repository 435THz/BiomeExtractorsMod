using System;
using BiomeExtractorsMod.Content.Items;
using BiomeExtractorsMod.Calamity.Content.Tiles;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using BiomeExtractorsMod.Common.Database;
using BiomeExtractorsMod.Calamity.Common;

namespace BiomeExtractorsMod.Calamity.Content.Items
{
    [ExtendsFromMod("CalamityMod")]
    [JITWhenModsEnabled("CalamityMod")]
    internal class AbyssalExtractorItem : BiomeExtractorItem
    {
        protected internal override int TileId => ModContent.TileType<AbyssalExtractorTile>();

        protected override ExtractorUpgradeKit UpgradeItemToCraftThis => throw new NotImplementedException();

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            BiomeExtractionSystem.Instance.AddTier(ExtractionTiers.SPECTRAL, $"{BiomeExtractorsMod.LocArticles}.Spectral", BiomeExtractorsMod.LocExtractorSuffix("Spectral"), delegate { return CalamityConfigs.Instance.SpectralExtractorRate; }, delegate { return CalamityConfigs.Instance.SpectralExtractorChance; }, delegate { return CalamityConfigs.Instance.SpectralExtractorAmount; }, delegate { return Mod.Assets.Request<Texture2D>("Calamity/Content/MapIcons/SpectralExtractorIcon"); });
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.value = Item.buyPrice(gold: 60); // sell at 12
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<ThermoresistantExtractorItem>())
                .AddIngredient(ModContent.ItemType<AbyssalUpgradeKit>())
                .Register();
        }
    }
}
