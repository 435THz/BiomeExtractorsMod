using BiomeExtractorsMod.Common.Configs;
using BiomeExtractorsMod.Content.Tiles;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.TileEntities
{
    public class BiomeExtractorEntInfernal : BiomeExtractorEnt
    {
        public override int Tier { get => (int)EnumTiers.INFERNAL; }
        public override int ExtractionRate { get => ModContent.GetInstance<ConfigCommon>().Tier3ExtractorRate; }
        public override int ExtractionChance { get => ModContent.GetInstance<ConfigCommon>().Tier3ExtractorChance; }
        protected override int TileType => ModContent.TileType<BiomeExtractorTileInfernal>();
    }
}