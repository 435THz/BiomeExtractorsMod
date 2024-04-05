using BiomeExtractorsMod.Common.Configs;
using BiomeExtractorsMod.Content.Tiles;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.TileEntities
{
    public class BiomeExtractorEntEthereal : BiomeExtractorEnt
    {
        public override int Tier { get => (int)EnumTiers.ETHEREAL; }
        public override int ExtractionRate { get => ModContent.GetInstance<ConfigCommon>().Tier7ExtractorRate; }
        public override int ExtractionChance { get => ModContent.GetInstance<ConfigCommon>().Tier7ExtractorChance; }
        protected override int TileType => ModContent.TileType<BiomeExtractorTileEthereal>();
    }
}