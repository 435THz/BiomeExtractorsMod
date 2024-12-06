using BiomeExtractorsMod.Content.TileEntities;
using BiomeExtractorsMod.Calamity.Common;
using BiomeExtractorsMod.Calamity.Content.Tiles;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;
using static BiomeExtractorsMod.Common.Database.BiomeExtractionSystem;
using BiomeExtractorsMod.Common.Database;

namespace BiomeExtractorsMod.Calamity.Content.TileEntities
{
    [ExtendsFromMod("CalamityMod")]
    [JITWhenModsEnabled("CalamityMod")]
    internal class SulphuricExtractorEnt : BiomeExtractorEnt
    {
        private readonly ExtractorIconOverride _iconOverride = new($"{BiomeExtractorsMod.LocExtractorSuffix("Sulphuric")}", delegate { return BiomeExtractorsMod.Instance.Assets.Request<Texture2D>("Calamity/Content/MapIcons/SulphuricExtractorIcon"); }, 0, 1);
        protected internal override ExtractorIconOverride IconOverride => _iconOverride;
        protected internal override string LocalName => Language.GetTextValue(BiomeExtractorsMod.LocExtractorSuffix("Sulphuric"));
        protected internal override int ExtractionRate => CalamityConfigs.Instance.SulphuricExtractorRate * 100 / (BiomeChecker.IsSubmerged((Position + Point16.NegativeOne).ToPoint()) ? 100 : CalamityConfigs.Instance.SulphuricExtractorDryEfficiency);
        protected internal override int ExtractionChance => CalamityConfigs.Instance.SulphuricExtractorChance;
        protected internal override int TileType => ModContent.TileType<SulphuricExtractorTile>();

        protected internal override ExtractionTier ExtractionTier => Instance.GetTier(ExtractionTiers.DEMONIC, true);
    }
}
