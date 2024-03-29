using BiomeExtractorsMod.Common.Configs;
using BiomeExtractorsMod.Content.Tiles;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.TileEntities
{
    public class BiomeExtractorEntBasic : BiomeExtractorEnt
    {
        public override int Tier { get => (int)EnumTiers.BASIC; }
        public override int ExtractionRate { get => ModContent.GetInstance<ExtractorConfig>().BasicExtractorRate; }
        public override int ExtractionChance { get => ModContent.GetInstance<ExtractorConfig>().BasicExtractorChance; }
        protected override int TileType => ModContent.TileType<BiomeExtractorTileBasic>();
    }
}