using BiomeExtractorsMod.Common.Database;
using BiomeExtractorsMod.Content.Tiles;
using Terraria.ModLoader;
using static BiomeExtractorsMod.Common.Database.BiomeExtractionSystem;

namespace BiomeExtractorsMod.Content.TileEntities
{
    public class BiomeExtractorEntEthereal : BiomeExtractorEnt
    {
        protected internal override ExtractionTier ExtractionTier => Instance.GetTier(ExtractionTiers.ETHEREAL, true);
        protected internal override int TileType => ModContent.TileType<BiomeExtractorTileEthereal>();
    }
}