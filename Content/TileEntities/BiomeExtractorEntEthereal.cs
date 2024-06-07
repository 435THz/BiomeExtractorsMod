using BiomeExtractorsMod.Common.Configs;
using BiomeExtractorsMod.Content.Tiles;
using Terraria.Localization;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.TileEntities
{
    public class BiomeExtractorEntEthereal : BiomeExtractorEnt
    {
        public override int Tier { get => (int)EnumTiers.ETHEREAL; }
        public override string LocalName { get => Language.GetTextValue($"{BiomeExtractorsMod.LocExtractorPrefix}Ethereal.DisplayName"); }
        public override int ExtractionRate { get => ModContent.GetInstance<ConfigCommon>().Tier7ExtractorRate; }
        public override int ExtractionChance { get => ModContent.GetInstance<ConfigCommon>().Tier7ExtractorChance; }
        protected override int TileType => ModContent.TileType<BiomeExtractorTileEthereal>();
    }
}