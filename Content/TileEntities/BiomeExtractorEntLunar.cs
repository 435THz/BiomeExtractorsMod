using BiomeExtractorsMod.Common.Configs;
using BiomeExtractorsMod.Content.Tiles;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.TileEntities
{
    public class BiomeExtractorEntLunar : BiomeExtractorEnt
    {
        public override int Tier { get => (int)EnumTiers.LUNAR; }
        public override int ExtractionRate { get => ModContent.GetInstance<ExtractorConfig>().Tier6ExtractorRate; }
        public override int ExtractionChance { get => ModContent.GetInstance<ExtractorConfig>().Tier6ExtractorChance; }
        protected override int TileType => ModContent.TileType<BiomeExtractorTileLunar>();
    }
}