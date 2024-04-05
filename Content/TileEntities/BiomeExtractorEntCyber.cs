using BiomeExtractorsMod.Common.Configs;
using BiomeExtractorsMod.Content.Tiles;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.TileEntities
{
    public class BiomeExtractorEntCyber : BiomeExtractorEnt
    {
        public override int Tier { get => (int)EnumTiers.CYBER; }
        public override int ExtractionRate { get => ModContent.GetInstance<ConfigCommon>().Tier5ExtractorRate; }
        public override int ExtractionChance { get => ModContent.GetInstance<ConfigCommon>().Tier5ExtractorChance; }
        protected override int TileType => ModContent.TileType<BiomeExtractorTileCyber>();
    }
}