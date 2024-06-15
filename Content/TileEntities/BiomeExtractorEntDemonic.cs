using BiomeExtractorsMod.Common.Configs;
using BiomeExtractorsMod.Content.Tiles;
using Terraria.Localization;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.TileEntities
{
    public class BiomeExtractorEntDemonic : BiomeExtractorEnt
    {
        protected internal override int Tier { get => (int)EnumTiers.DEMONIC; }
        protected internal override string LocalName { get => Language.GetTextValue($"{BiomeExtractorsMod.LocExtractorPrefix}Corruption.DisplayName"); }
        protected internal override int ExtractionRate { get => ModContent.GetInstance<ConfigCommon>().Tier2ExtractorRate; }
        protected internal override int ExtractionChance { get => ModContent.GetInstance<ConfigCommon>().Tier2ExtractorChance; }
        protected override int TileType => ModContent.TileType<BiomeExtractorTileDemonic>();
    }
}