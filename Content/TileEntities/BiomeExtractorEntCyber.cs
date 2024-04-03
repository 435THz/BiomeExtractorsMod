using BiomeExtractorsMod.Common.Configs;
using BiomeExtractorsMod.Content.Tiles;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.TileEntities
{
    public class BiomeExtractorEntCyber : BiomeExtractorEnt
    {
        public override int Tier { get => (int)EnumTiers.CYBER; }
        public override int ExtractionRate { get => ModContent.GetInstance<ExtractorConfig>().Tier5ExtractorRate; }
        public override int ExtractionChance { get => ModContent.GetInstance<ExtractorConfig>().Tier5ExtractorChance; }
        protected override int TileType => ModContent.TileType<BiomeExtractorTileCyber>();
    }
}