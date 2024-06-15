using BiomeExtractorsMod.Common.Configs;
using BiomeExtractorsMod.Content.Tiles;
using Terraria.Localization;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.TileEntities
{
    public class BiomeExtractorEntLunar : BiomeExtractorEnt
    {
        protected internal override int Tier { get => (int)EnumTiers.LUNAR; }
        protected internal override string LocalName { get => Language.GetTextValue($"{BiomeExtractorsMod.LocExtractorPrefix}Lunar.DisplayName"); }
        protected internal override int ExtractionRate { get => ModContent.GetInstance<ConfigCommon>().Tier6ExtractorRate; }
        protected internal override int ExtractionChance { get => ModContent.GetInstance<ConfigCommon>().Tier6ExtractorChance; }
        protected override int TileType => ModContent.TileType<BiomeExtractorTileLunar>();
    }
}