using BiomeExtractorsMod.Common.Configs;
using BiomeExtractorsMod.Content.Tiles;
using Terraria.Localization;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.TileEntities
{
    public class BiomeExtractorEntBasic : BiomeExtractorEnt
    {
        public override int Tier { get => (int)EnumTiers.BASIC; }
        public override string LocalName { get => Language.GetTextValue($"{BiomeExtractorsMod.LocExtractorPrefix}Iron.DisplayName"); }
        public override int ExtractionRate { get => ModContent.GetInstance<ConfigCommon>().Tier1ExtractorRate; }
        public override int ExtractionChance { get => ModContent.GetInstance<ConfigCommon>().Tier1ExtractorChance; }
        protected override int TileType => ModContent.TileType<BiomeExtractorTileBasic>();
    }
}