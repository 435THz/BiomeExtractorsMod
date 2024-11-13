using BiomeExtractorsMod.Common.Database;
using BiomeExtractorsMod.Content.Tiles;
using Terraria.ModLoader;
using static BiomeExtractorsMod.Common.Database.BiomeExtractionSystem;

namespace BiomeExtractorsMod.Content.TileEntities
{
    public class BiomeExtractorEntInfernal : BiomeExtractorEnt
    {
        protected internal override ExtractionTier ExtractionTier => Instance.GetTier((int)EnumTiers.INFERNAL, true);
        protected internal override int TileType => ModContent.TileType<BiomeExtractorTileInfernal>();
    }
}