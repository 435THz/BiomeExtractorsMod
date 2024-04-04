using BiomeExtractorsMod.Common.Configs;
using BiomeExtractorsMod.Content.Tiles;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.TileEntities
{
    public class BiomeExtractorEntSteampunk : BiomeExtractorEnt
    {
        public override int Tier { get => (int)EnumTiers.STEAMPUNK; }
        public override int ExtractionRate { get => ModContent.GetInstance<ExtractorConfig>().Tier4ExtractorRate; }
        public override int ExtractionChance { get => ModContent.GetInstance<ExtractorConfig>().Tier4ExtractorChance; }
        protected override int TileType => ModContent.TileType<BiomeExtractorTileSteampunk>();
    }
}