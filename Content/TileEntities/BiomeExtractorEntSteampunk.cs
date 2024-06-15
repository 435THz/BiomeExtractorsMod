using BiomeExtractorsMod.Common.Configs;
using BiomeExtractorsMod.Content.Tiles;
using Terraria.Localization;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.TileEntities
{
    public class BiomeExtractorEntSteampunk : BiomeExtractorEnt
    {
        protected internal override int Tier { get => (int)EnumTiers.STEAMPUNK; }
        protected internal override string LocalName { get => Language.GetTextValue($"{BiomeExtractorsMod.LocExtractorPrefix}Adamantite.DisplayName"); }
        protected internal override int ExtractionRate { get => ModContent.GetInstance<ConfigCommon>().Tier4ExtractorRate; }
        protected internal override int ExtractionChance { get => ModContent.GetInstance<ConfigCommon>().Tier4ExtractorChance; }
        protected override int TileType => ModContent.TileType<BiomeExtractorTileSteampunk>();
    }
}