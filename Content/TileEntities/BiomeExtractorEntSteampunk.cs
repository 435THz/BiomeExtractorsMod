using BiomeExtractorsMod.Content.Tiles;
using Terraria.ModLoader;
using static BiomeExtractorsMod.Common.Systems.BiomeExtractionSystem;

namespace BiomeExtractorsMod.Content.TileEntities
{
    public class BiomeExtractorEntSteampunk : BiomeExtractorEnt
    {
        protected internal override ExtractionTier ExtractionTier => Instance.GetTier((int)EnumTiers.STEAMPUNK, true);
        protected internal override int TileType => ModContent.TileType<BiomeExtractorTileSteampunk>();
    }
}