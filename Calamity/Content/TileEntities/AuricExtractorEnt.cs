using BiomeExtractorsMod.Content.TileEntities;
using BiomeExtractorsMod.Calamity.Content.Tiles;
using Terraria.ModLoader;
using static BiomeExtractorsMod.Common.Database.BiomeExtractionSystem;
using BiomeExtractorsMod.Common.Database;

namespace BiomeExtractorsMod.Calamity.Content.TileEntities
{
    [ExtendsFromMod("CalamityMod")]
    [JITWhenModsEnabled("CalamityMod")]
    internal class AuricExtractorEnt : BiomeExtractorEnt
    {
        protected internal override int TileType => ModContent.TileType<AuricExtractorTile>();
        protected internal override ExtractionTier ExtractionTier => Instance.GetTier(ExtractionTiers.AURIC, true);
    }
}
