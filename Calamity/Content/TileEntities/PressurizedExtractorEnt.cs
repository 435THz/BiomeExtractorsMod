﻿using BiomeExtractorsMod.Calamity.Common;
using BiomeExtractorsMod.Calamity.Content.Tiles;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Localization;
using Terraria.ModLoader;
using static BiomeExtractorsMod.Common.Database.BiomeExtractionSystem;
using BiomeExtractorsMod.Common.Database;

namespace BiomeExtractorsMod.Calamity.Content.TileEntities
{
    [ExtendsFromMod("CalamityMod")]
    [JITWhenModsEnabled("CalamityMod")]
    internal class PressurizedExtractorEnt : BiomeExtractorEntAbyss
    {
        private readonly ExtractorIconOverride _iconOverride = new($"{BiomeExtractorsMod.LocExtractorSuffix("Pressurized")}", delegate { return BiomeExtractorsMod.Instance.Assets.Request<Texture2D>("Calamity/Content/MapIcons/PressurizedExtractorIcon"); }, 0, 1);
        protected internal override ExtractorIconOverride IconOverride => _iconOverride;
        protected internal override string LocalName => Language.GetTextValue(BiomeExtractorsMod.LocExtractorSuffix("Pressurized"));
        protected internal override int ExtractionRate => CalamityConfigs.Instance.PressurizedExtractorRate;
        protected internal override int ExtractionChance => CalamityConfigs.Instance.PressurizedExtractorChance;
        protected internal override int ExtractionAmount => CalamityConfigs.Instance.PressurizedExtractorAmount;
        protected internal override int TileType => ModContent.TileType<PressurizedExtractorTile>();

        protected internal override ExtractionTier ExtractionTier => Instance.GetTier(ExtractionTiers.INFERNAL, true);
    }
}
