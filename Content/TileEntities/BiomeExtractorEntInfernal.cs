using BiomeExtractorsMod.Common.Configs;
using BiomeExtractorsMod.Content.Tiles;
using Terraria.Localization;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.TileEntities
{
    public class BiomeExtractorEntInfernal : BiomeExtractorEnt
    {
        protected internal override int Tier { get => (int)EnumTiers.INFERNAL; }
        protected internal override string LocalName { get => Language.GetTextValue($"{BiomeExtractorsMod.LocExtractorPrefix}Infernal.DisplayName"); }
        protected internal override int ExtractionRate { get => ModContent.GetInstance<ConfigCommon>().Tier3ExtractorRate; }
        protected internal override int ExtractionChance { get => ModContent.GetInstance<ConfigCommon>().Tier3ExtractorChance; }
        protected override int TileType => ModContent.TileType<BiomeExtractorTileInfernal>();
    }
}