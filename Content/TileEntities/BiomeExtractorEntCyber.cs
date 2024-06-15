using BiomeExtractorsMod.Common.Configs;
using BiomeExtractorsMod.Content.Tiles;
using Terraria.Localization;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.TileEntities
{
    public class BiomeExtractorEntCyber : BiomeExtractorEnt
    {
        protected internal override int Tier { get => (int)EnumTiers.CYBER; }
        protected internal override string LocalName { get => Language.GetTextValue($"{BiomeExtractorsMod.LocExtractorPrefix}Cyber.DisplayName"); }
        protected internal override int ExtractionRate { get => ModContent.GetInstance<ConfigCommon>().Tier5ExtractorRate; }
        protected internal override int ExtractionChance { get => ModContent.GetInstance<ConfigCommon>().Tier5ExtractorChance; }
        protected override int TileType => ModContent.TileType<BiomeExtractorTileCyber>();
    }
}