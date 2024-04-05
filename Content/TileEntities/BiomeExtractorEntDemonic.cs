using BiomeExtractorsMod.Common.Configs;
using BiomeExtractorsMod.Content.Tiles;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.TileEntities
{
    public class BiomeExtractorEntDemonic : BiomeExtractorEnt
    {
        public override int Tier { get => (int)EnumTiers.DEMONIC; }
        public override int ExtractionRate { get => ModContent.GetInstance<ConfigCommon>().Tier2ExtractorRate; }
        public override int ExtractionChance { get => ModContent.GetInstance<ConfigCommon>().Tier2ExtractorChance; }
        protected override int TileType => ModContent.TileType<BiomeExtractorTileDemonic>();
    }
}