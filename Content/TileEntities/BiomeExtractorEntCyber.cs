using BiomeExtractorsMod.Common.Configs;
using BiomeExtractorsMod.Content.Tiles;
using Terraria.Localization;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.TileEntities
{
    public class BiomeExtractorEntCyber : BiomeExtractorEnt
    {
        public override int Tier { get => (int)EnumTiers.CYBER; }
        public override string LocalName { get => Language.GetTextValue($"{BiomeExtractorsMod.LocExtractorPrefix}Cyber.DisplayName"); }
        public override int ExtractionRate { get => ModContent.GetInstance<ConfigCommon>().Tier5ExtractorRate; }
        public override int ExtractionChance { get => ModContent.GetInstance<ConfigCommon>().Tier5ExtractorChance; }
        protected override int TileType => ModContent.TileType<BiomeExtractorTileCyber>();
    }
}