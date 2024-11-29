using BiomeExtractorsMod.Content.TileEntities;
using BiomeExtractorsMod.Calamity.Content.Tiles;
using Terraria.ModLoader;
using static BiomeExtractorsMod.Common.Database.BiomeExtractionSystem;
using BiomeExtractorsMod.Common.Database;

namespace BiomeExtractorsMod.Calamity.Content.TileEntities
{
    [ExtendsFromMod("CalamityMod")]
    [JITWhenModsEnabled("CalamityMod")]
    internal class SpectralExtractorEnt : BiomeExtractorEnt
    {
        protected internal override int TileType => ModContent.TileType<SpectralExtractorTile>();
        protected internal override ExtractionTier ExtractionTier => Instance.GetTier(ExtractionTiers.SPECTRAL, true);
    }
}
